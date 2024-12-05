using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Interfaces;
using Managers;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.Zones;
using Units.Stages.Enums;
using Units.Stages.Modules;
using Units.Stages.Units.Buildings.Abstract;
using Units.Stages.Units.Buildings.Enums;
using Units.Stages.Units.Buildings.Units;
using Units.Stages.Units.Creatures.Enums;
using Units.Stages.Units.Creatures.Units;
using Units.Stages.Units.HuntingZones;
using Units.Stages.Units.Items.Enums;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

namespace Units.Stages.Controllers
{
    public interface IVillageZoneController :
        IRegisterReference<ICreatureController, IBuildingController, HuntingZoneController, StageCustomSettings>, IInitializable
    {
        public IPlayer Player { get; }
        public Action<IPlayer, bool> OnRegisterPlayer { get; set; }
    }

    [Serializable]
    public struct VillageSpawnData
    {
        [Header("=== Village Position ===")]
        public Transform VillageSpawner;
        public Transform PlayerSpawner;
        public List<Transform> GuestSpawner;
        public List<Transform> ZombieSpawner;
    }

    public class VillageZoneController : MonoBehaviour, IVillageZoneController
    {
        [SerializeField] private VillageSpawnData _villageSpawnData;
        public VillageSpawnData VillageSpawnData => _villageSpawnData;
        
        public Action<IPlayer, bool> OnRegisterPlayer { get; set; }
        public IPlayer Player { get; private set; }
        
        private readonly Queue<Tuple<string, Transform>> _deliveryTargetQueue = new();
        private readonly HashSet<IDeliveryMan> currentSpawnedDeliveryMans = new();
        private readonly HashSet<IGuest> currentSpawnedGuests = new();
        private readonly HashSet<IHunter> currentSpawnedHunters = new();
        private IBuildingController _buildingController;
        private ICreatureController _creatureController;
        private float _guestSpawnCheckTime;
        private float _guestSpawnElapsedTime;
        private GuestSpawnZoneDataSo _guestSpawnZoneDataSo;
        private IHuntingZoneController _huntingZoneController;
        private StageCustomSettings _stageCustomSettings;

        private const float customerRushButtonActivatorMaxTime = 50f;
        // private const float superHunterButtonActivatorMaxTime = ;
        private float customerRushButtonActivatorTimer = 0f;
        private float superHunterButtonActivatorTimer = 0f;

        private Dictionary<HuntingZone, EActiveStatus> currentHuntingZones = new();
        private float _guestMaxCount => _stageCustomSettings.MaxGuestCount;

        public void RegisterReference(
            ICreatureController creatureController,
            IBuildingController buildingController,
            HuntingZoneController huntingZoneController,
            StageCustomSettings stageCustomSettings)
        {
            _creatureController = creatureController;
            _buildingController = buildingController;
            _huntingZoneController = huntingZoneController;

            _guestSpawnZoneDataSo = DataManager.Instance.GuestSpawnZoneDataSo;
            _stageCustomSettings = stageCustomSettings;

            currentHuntingZones = VolatileDataManager.Instance.HuntingZoneActiveStatuses;

            foreach (IGuest guest in ObjectPoolManager.Instance.GetAllObjects<IGuest>("GuestPool"))
            {
                guest.Transform.position = _villageSpawnData.GuestSpawner[0].transform.position;
            }

            if (_buildingController.Buildings.ContainsKey($"{EBuildingType.DeliveryLodging}"))
            {
                foreach (IDeliveryMan deliveryMan in ObjectPoolManager.Instance.GetAllObjects<IDeliveryMan>("DeliveryManPool"))
                {
                    deliveryMan.Transform.position = _buildingController.Buildings[$"{EBuildingType.DeliveryLodging}"].transform.position;
                }
            }

            if (_buildingController.Buildings.ContainsKey($"{EBuildingType.WareHouse}"))
            {
                foreach (IHunter hunter in ObjectPoolManager.Instance.GetAllObjects<IHunter>("HunterPool"))
                {
                    hunter.Transform.position = _buildingController.Buildings[$"{EBuildingType.WareHouse}"].transform.position;
                }
            }

            InstantiateLevels();
        }

        private void InstantiateLevels()
        {
            GameObject prefab = DataManager.Instance.levelPrefabSo.Levels[GameManager.Instance.ES3Saver.CurrentStageLevel - 1].Village;
            GameObject village = Instantiate(prefab, _villageSpawnData.VillageSpawner);
            village.transform.localPosition = Vector3.zero;
            
            prefab = DataManager.Instance.levelPrefabSo.GuestSpawner;

            foreach (GameObject guestSpawner in _villageSpawnData.GuestSpawner.Select(t => Instantiate(prefab, t)))
            {
                guestSpawner.transform.localPosition = Vector3.zero;
            }
            
            prefab = DataManager.Instance.levelPrefabSo.PlayerSpawner;
            GameObject playerSpawner = Instantiate(prefab, _villageSpawnData.PlayerSpawner);
            playerSpawner.transform.localPosition = Vector3.zero;
        }

        public void Initialize()
        {
            SpawnPlayer();
        }
        
        private void Update()
        {
#if UNITY_EDITOR
            // TODO : Cheat Code
            if (Input.GetKeyDown(KeyCode.E))
                if (VolatileDataManager.Instance.CurrentActiveMaterials.Count > 0)
                {
                    var randomPosition = new Random().Next(_villageSpawnData.GuestSpawner.Count);
                    IGuest guest = _creatureController.GetGuest(_villageSpawnData.GuestSpawner[randomPosition].position, ReturnGuest);
                    guest.SetTargetPurchaseQuantity(1);
                    guest.SetDestinations(GetRandomDestinationForGuest());

                    currentSpawnedGuests.Add(guest);
                }
#endif
            
            if (GameManager.Instance.ES3Saver.InitialTutorialClear)
            {
                SpawnDeliveryMan();
                SetDeliveryManDestination();
                SpawnHunter();
                SetHunterDestination();
                SpawnGuests();
                PopUpGuestRush();
                PopUpSuperHunter();
                SpawnPopUpRewardGuestRush();
            }
            
            SpawnMonster();
  
            _huntingZoneController.SendDroppedItem(currentSpawnedHunters);
            _huntingZoneController.ControlDroppedItem();
        }

        private void SpawnPopUpRewardGuestRush()
        {
            if (VolatileDataManager.Instance.CustomerTrigger)
            {
                VolatileDataManager.Instance.CustomerTrigger = false;
                StartCoroutine(SpawnGuestsWithDelay(20, 0.1f)); // 30명의 손님을 0.5초 간격으로 생성
                customerRushButtonActivatorTimer = customerRushButtonActivatorMaxTime;
            }
        }

        private IEnumerator SpawnGuestsWithDelay(int guestCount, float delay)
        {
            for (var i = 0; i < guestCount; i++)
            {
                if (VolatileDataManager.Instance.CurrentActiveMaterials.Count > 0)
                {
                    var randomPosition = new Random().Next(_villageSpawnData.GuestSpawner.Count);
                    IGuest guest = _creatureController.GetGuest(_villageSpawnData.GuestSpawner[randomPosition].position, ReturnGuest);
                    guest.SetTargetPurchaseQuantity(1);
                    guest.SetDestinations(GetRandomDestinationForGuest());

                    currentSpawnedGuests.Add(guest);
                }

                yield return new WaitForSeconds(delay); // 0.5초 대기
            }
        }

        private void PopUpGuestRush()
        {
            if (customerRushButtonActivatorTimer <= 0 && VolatileDataManager.Instance.CustomerTrigger == false)
            {
                var materialTypes = new[] { EMaterialType.A, EMaterialType.B, EMaterialType.C, EMaterialType.D };

                var currentStandProductCount = materialTypes
                    .Where(material => GameManager.Instance.ES3Saver.BuildingOutputItems.ContainsKey($"{EBuildingType.StandA}_{material}"))
                    .Sum(material => GameManager.Instance.ES3Saver.BuildingOutputItems[$"{EBuildingType.StandA}_{material}"].Values.Sum());

                if (currentStandProductCount >= 30)
                {
                    UIManager.Instance.UI_Panel_Main.UI_Panel_MainButtons.UI_Button_CustomerWave.Activate(() =>
                    {
                        customerRushButtonActivatorTimer = customerRushButtonActivatorMaxTime;
                    });
                }
            }
            else
            {
                customerRushButtonActivatorTimer -= Time.deltaTime;
            }
        }
        
        private void PopUpSuperHunter()
        {
            
        }

        private void SetDeliveryManDestination()
        {
            // 현재 생성된 배달원 리스트가 비어있으면 함수를 종료한다.
            if (currentSpawnedDeliveryMans.Count <= 0) return;

            // 배달 대상 큐 초기화
            _deliveryTargetQueue.Clear();

            // 활성화된 주방(Kitchen) 중 재고가 있는 건물을 배달 대상 큐에 추가
            foreach (KeyValuePair<string, BuildingZone> building in _buildingController.Buildings)
            {
                if (building.Value is Kitchen { ActiveStatus: EActiveStatus.Active, IsAnyItemOnInventory: true } kitchen)
                {
                    var target = new Tuple<string, Transform>(kitchen.BuildingKey, kitchen.TradeZoneNpcTransform);

                    if (!_deliveryTargetQueue.Contains(target))
                    {
                        _deliveryTargetQueue.Enqueue(target); // 중복되지 않으면 큐에 추가   
                    }
                }   
            }

            // 배달 대상이 없을 경우
            if (_deliveryTargetQueue.Count == 0)
            {
                // 모든 배달원의 상태를 확인하여 "명령 없음" 또는 "대기" 상태일 경우 기본 목적지를 설정
                foreach (IDeliveryMan deliveryMan in currentSpawnedDeliveryMans)
                {
                    if (deliveryMan.CommandState is CommandState.NoOrder or CommandState.Standby)
                    {
                        // 기본 목적지는 특정 키에 해당하는 주방으로 설정
                        var defaultDestinationKey = $"{EBuildingType.KitchenA}_{EMaterialType.A}";
                        var defaultDestination = new Tuple<string, Transform>(
                            defaultDestinationKey,
                            _buildingController.Buildings[defaultDestinationKey].gameObject.transform);

                        // 기본 목적지 설정 및 상태를 대기로 변경
                        deliveryMan.SetDestinations(defaultDestination);
                        deliveryMan.CommandState = CommandState.Standby;
                    }   
                }
            }
            else
            {
                // 배달 대상이 있을 경우
                foreach (IDeliveryMan deliveryMan in currentSpawnedDeliveryMans)
                    if (deliveryMan.CommandState is CommandState.NoOrder or CommandState.Standby)
                    {
                        // 배달 대상 큐에서 첫 번째 항목을 가져와 배달원에게 설정
                        Tuple<string, Transform> target = _deliveryTargetQueue.Dequeue();
                        deliveryMan.SetDestinations(target);
                        deliveryMan.CommandState = CommandState.MoveTo; // 상태를 이동 중으로 변경
                    }
            }

            // 배달원들의 상태를 확인하고 인벤토리가 가득 찼을 경우
            foreach (IDeliveryMan deliveryMan in currentSpawnedDeliveryMans)
            {
                if (deliveryMan.IsInventoryFull() && deliveryMan.CommandState == CommandState.MoveTo)
                {
                    // 현재 목적지 정보를 가져와 파싱
                    Tuple<string, Transform> destination = deliveryMan.GetDestination();
                    (EBuildingType?, EMaterialType?) parsedKey = ParserModule.ParseStringToEnum<EBuildingType, EMaterialType>(destination.Item1);

                    // 가공된 상품을 판매할 Stand 건물의 키와 위치를 생성
                    if (parsedKey.Item1 != null)
                    {
                        var targetBuilding = parsedKey.Item1 == EBuildingType.KitchenA
                            ? EBuildingType.StandA
                            : EBuildingType.StandB;
                        var standKey =  $"{targetBuilding}_{parsedKey.Item2}";
                        var standDestination = new Tuple<string, Transform>(
                            standKey,
                            _buildingController.Buildings[standKey].gameObject.transform);

                        // 목적지를 Stand 건물로 설정하고 상태를 "배송 중"으로 변경
                        deliveryMan.SetDestinations(standDestination);
                        deliveryMan.CommandState = CommandState.Deliver;
                    }
                }
                // 배달원의 인벤토리가 비었고, 상태가 "배송 중"일 경우 명령 없음으로 상태 변경
                else if (!deliveryMan.HaveAnyItem() && deliveryMan.CommandState == CommandState.Deliver)
                {
                    deliveryMan.CommandState = CommandState.NoOrder;
                }
            }
            
            // // 목표 건물 유효성 검사
            // foreach (IDeliveryMan deliveryMan in currentSpawnedDeliveryMans)
            // {
            //     if (deliveryMan.CommandState is CommandState.MoveTo or CommandState.NoOrder && !deliveryMan.HaveAnyItem())
            //     {
            //         Tuple<string, Transform> destination = deliveryMan.GetDestination();
            //         var targetBuilding = _buildingController.Buildings[destination.Item1];
            //         
            //         if (targetBuilding is Kitchen { IsAnyItemOnInventory: false })
            //         {
            //             // 기본 목적지는 특정 키에 해당하는 주방으로 설정
            //             var defaultDestinationKey = $"{EBuildingType.KitchenA}_{EMaterialType.A}";
            //             var defaultDestination = new Tuple<string, Transform>(
            //                 defaultDestinationKey,
            //                 _buildingController.Buildings[defaultDestinationKey].gameObject.transform);
            //
            //             // 기본 목적지 설정 및 상태를 대기로 변경
            //             deliveryMan.SetDestinations(defaultDestination);
            //             deliveryMan.CommandState = CommandState.Standby;
            //         }
            //     }
            // }
        }

        private void SetHunterDestination()
        {
            // 1. 헌터가 없으면 로직 종료
            if (currentSpawnedHunters.Count <= 0) return;

            // 2. 현재 활성화된 모든 헌터를 대상으로 타겟 설정
            foreach (IHunter hunter in currentSpawnedHunters)
            {
                if (!hunter.IsInventoryFull() && hunter.CommandState == CommandState.Deliver)
                    hunter.CommandState = CommandState.NoOrder;

                // 2-1. 헌터 인벤토리가 가득 찬 경우 복귀
                if (hunter.IsInventoryFull() && hunter.CommandState != CommandState.Deliver)
                {
                    hunter.InactivateWeapon();
                    hunter.SetDestinations(new Tuple<string, Transform>($"{EBuildingType.WareHouse}",
                        _buildingController.Buildings[$"{EBuildingType.WareHouse}"].transform)); // 복귀 위치 설정
                    hunter.CommandState = CommandState.Deliver;
                    continue; // 다른 조건을 확인하지 않고 다음 헌터로 넘어감
                }

                // 2-2. 헌터가 NoOrder 상태라면 새로운 타겟 몬스터 탐색
                if (hunter.CommandState == CommandState.NoOrder)
                {
                    IMonster closestMonster = null;
                    var closestDistance = float.MaxValue;

                    // 가장 가까운 활성화된 몬스터 찾기
                    foreach (KeyValuePair<HuntingZone, EActiveStatus> huntingZone in currentHuntingZones)
                        if (huntingZone.Value == EActiveStatus.Active)
                            foreach (IMonster monster in huntingZone.Key.CurrentSpawnedMonsters)
                                if (monster.Transform.gameObject.activeInHierarchy)
                                {
                                    var distance = Vector3.Distance(hunter.Transform.position,
                                        monster.Transform.position);
                                    if (distance < closestDistance)
                                    {
                                        closestDistance = distance;
                                        closestMonster = monster;
                                    }
                                }

                    // 가장 가까운 몬스터를 타겟으로 지정
                    if (closestMonster != null)
                    {
                        hunter.SetDestinations(new Tuple<string, Transform>($"{ECreatureType.Monster}", closestMonster.Transform));
                        hunter.CommandState = CommandState.MoveTo;
                    }
                }

                // 2-3. 추적 중인 타겟 몬스터가 비활성화되었는지 확인
                if (hunter.CommandState == CommandState.MoveTo)
                {
                    Transform currentTarget = hunter.GetDestination().Item2;
                    if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy)
                        // 타겟이 비활성화되었으므로 헌터를 NoOrder 상태로 전환하여 다른 타겟을 대기
                        hunter.CommandState = CommandState.NoOrder;
                }
            }
        }

        private void SpawnDeliveryMan()
        {
            if (_buildingController.Buildings.TryGetValue($"{EBuildingType.DeliveryLodging}", out BuildingZone buildingZone))
            {
                if (buildingZone is DeliveryLodging deliveryLodging)
                {
                    deliveryLodging.SpawnDeliveryMan(_creatureController, currentSpawnedDeliveryMans);
                }   
            }
        }

        private void SpawnHunter()
        {
            if (_buildingController.Buildings.TryGetValue($"{EBuildingType.WareHouse}", out BuildingZone buildingZone))
            {
                if (buildingZone is WareHouse wareHouse)
                {
                    wareHouse.SpawnHunter(_creatureController, currentSpawnedHunters);                    
                }
            }
        }

        private void SpawnMonster()
        {
            _huntingZoneController.SpawnMonsters();
        }

        private void SpawnPlayer()
        {
            Player = _creatureController.GetPlayer();
            Player.Initialize(_villageSpawnData.PlayerSpawner.position, ReturnPlayer);

            OnRegisterPlayer?.Invoke(Player, true);
        }

        private void ReturnPlayer()
        {
            OnRegisterPlayer?.Invoke(Player, false);
        }

        private void SpawnGuests()
        {
            if (currentSpawnedGuests.Count < _guestMaxCount && VolatileDataManager.Instance.CurrentActiveMaterials.Count > 0)
            {
                if (_guestSpawnCheckTime == 0f)
                    _guestSpawnCheckTime = UnityEngine.Random.Range(_guestSpawnZoneDataSo.guestSpawnMinimumTime,
                        _guestSpawnZoneDataSo.guestSpawnMaximumTime);

                _guestSpawnElapsedTime += Time.deltaTime;

                if (_guestSpawnElapsedTime >= _guestSpawnCheckTime)
                {
                    var randomPosition = UnityEngine.Random.Range(0, _villageSpawnData.GuestSpawner.Count);
                    Vector3 spawnPosition = _villageSpawnData.GuestSpawner[randomPosition].position;

                    if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 5f, NavMesh.AllAreas))
                    {
                        IGuest guest = _creatureController.GetGuest(hit.position, ReturnGuest);
                        guest.SetTargetPurchaseQuantity(1);
                        guest.SetDestinations(GetRandomDestinationForGuest());

                        currentSpawnedGuests.Add(guest);
                    }

                    _guestSpawnElapsedTime = 0f;
                    _guestSpawnCheckTime = 0f;
                }
            }
        }


        private void ReturnGuest(IGuest guest)
        {
            currentSpawnedGuests.Remove(guest);
        }

        private List<Tuple<string, Transform>> GetRandomDestinationForGuest()
        {
            // 활성화된 Stand 건물을 필터링
            List<Tuple<string, Transform>> activeStands = _buildingController.Buildings
                .Where(building => building.Value is Stand { ActiveStatus: EActiveStatus.Active })
                .Select(building => new Tuple<string, Transform>(building.Key, building.Value.gameObject.transform))
                .ToList();

            // 활성화된 Stand가 없으면 기본값으로 반환
            if (activeStands.Count == 0)
            {
                return new List<Tuple<string, Transform>>
                {
                    new(string.Empty, _villageSpawnData.GuestSpawner[0]) // Default fallback destination
                };
            }

            // 활성화된 Stand 중 랜덤 선택
            var randomIndex = UnityEngine.Random.Range(0, activeStands.Count);
            Tuple<string, Transform> randomStand = activeStands[randomIndex];

            // ManagementDesk 키 생성
            var managementDeskKey = ParserModule.ParseEnumToString(EBuildingType.ManagementDesk);
            Transform managementDeskTransform = _buildingController.Buildings[managementDeskKey].transform;

            // GuestSpawner 중 랜덤 위치 선택
            var randomGuestSpawnerIndex = UnityEngine.Random.Range(0, _villageSpawnData.GuestSpawner.Count);
            Transform randomGuestSpawner = _villageSpawnData.GuestSpawner[randomGuestSpawnerIndex];

            // 목적지 리스트 구성
            var destinations = new List<Tuple<string, Transform>>
            {
                randomStand, // 랜덤으로 선택된 활성화된 Stand
                new(managementDeskKey, managementDeskTransform), // ManagementDesk
                new(string.Empty, randomGuestSpawner) // 랜덤 GuestSpawner
            };

            return destinations;
        }
    }
}