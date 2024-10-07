using System.Collections.Generic;
using System.Linq;
using Enums;
using Units.Buildings.Abstract;
using Units.Creatures.Abstract;
using Units.Creatures.Units.Players;
using UnityEngine;
using Interfaces;
using ScriptableObjects.Scripts;
using UnityEditor;

namespace Units.Buildings.Controllers
{
    public interface IBuildingController : IReferenceRegisterable<List<EMaterialType>>, IInitializable
    {
        // 빌딩을 스폰하는 메서드
        public void SpawnBuilding(List<SpawnData> buildingSpawnDatas, Grid grid);

        // 빌딩 큐에 플레이어나 NPC를 추가하는 메서드
        public void AddToBuildingQueue(int buildingInstanceID, BaseCreature creature);

        // 빌딩 큐에서 플레이어나 NPC를 제거하는 메서드
        public void RemoveFromBuildingQueue(int buildingInstanceID, BaseCreature creature);

        // 빌딩 ID로 빌딩을 가져오는 메서드
        BaseBuilding GetBuildingById(int id);
    }

    public class BuildingController : MonoBehaviour, IBuildingController
    {
        // 빌딩과 관련된 데이터를 저장할 딕셔너리 및 큐
        private Dictionary<int, Queue<BaseCreature>> _buildingQueues;
        private Dictionary<int, BaseBuilding> _buildings;

        private List<EMaterialType> _materials;

        private IBuildingFactory _buildingFactory;

        // BuildingFactory를 주입받는 생성자 추가
        public BuildingController(IBuildingFactory buildingFactory)
        {
            _buildingFactory = buildingFactory;
        }
        
        // 재료 목록을 등록
        public void RegisterReference(List<EMaterialType> materials)
        {
            _materials = materials;

            _buildingQueues = new Dictionary<int, Queue<BaseCreature>>();
            _buildings = new Dictionary<int, BaseBuilding>();
        }
        
        // 초기화 메서드
        public void Initialize()
        {
            // 필요한 초기화 로직이 있을 경우 추가 가능
        }

        // 빌딩을 스폰하는 메서드 (빌딩 생성 후 딕셔너리에 등록)
        public void SpawnBuilding(List<SpawnData> buildingSpawnDatas, Grid grid)
        {
            foreach (SpawnData buildingSpawnData in buildingSpawnDatas)
            {
                GameObject buildingObject = Instantiate(buildingSpawnData.prefab, grid.transform);
                var building = buildingObject.GetComponent<BaseBuilding>();
                
                if (building != null)
                {
                    RegisterInstanceID(building);
                }
            }
        }

        // 빌딩을 큐에 추가
        public void AddToBuildingQueue(int buildingInstanceID, BaseCreature creature)
        {
            Debug.Log($"InstanceID : {buildingInstanceID} 빌딩과 {creature.gameObject.name} 바인딩");
            
            if (!_buildingQueues.ContainsKey(buildingInstanceID))
            {
                _buildingQueues[buildingInstanceID] = new Queue<BaseCreature>();
            }
            _buildingQueues[buildingInstanceID].Enqueue(creature);  // 큐에 Creature 추가
        }

        // 빌딩 큐에서 Creature를 제거
        public void RemoveFromBuildingQueue(int buildingInstanceID, BaseCreature creature)
        {
            Debug.Log($"InstanceID : {buildingInstanceID} 빌딩과 {creature.gameObject.name} 바인딩 해제");
            
            if (_buildingQueues.ContainsKey(buildingInstanceID))
            {
                var queue = _buildingQueues[buildingInstanceID];
                if (queue.Contains(creature))
                {
                    var newQueue = new Queue<BaseCreature>(queue);
                    newQueue = new Queue<BaseCreature>(newQueue.Where(c => c != creature));
                    _buildingQueues[buildingInstanceID] = newQueue;  // 큐 갱신
                }
            }
        }

        // 빌딩 ID로 빌딩을 가져오는 메서드
        public BaseBuilding GetBuildingById(int id)
        {
            return _buildings.GetValueOrDefault(id);
        }

        // 빌딩의 큐를 처리하는 Update 메서드
        private void Update()
        {
            if (_buildingQueues.Values.Count > 0)
            {
                foreach (KeyValuePair<int, Queue<BaseCreature>> buildingQueue in _buildingQueues)
                {
                    if (buildingQueue.Value.Count > 0)
                    {
                        ProcessBuildingQueue(buildingQueue.Key, buildingQueue.Value);
                    }
                }
            }
        }

        // 빌딩의 큐를 처리하는 메서드
        private void ProcessBuildingQueue(int buildingInstanceID, Queue<BaseCreature> queue)
        {
            if (_buildings.TryGetValue(buildingInstanceID, out var building))
            {
                var creature = queue.Peek();  // 큐에서 첫 번째 Creature 확인
                var player = creature as Player;

                if (player != null)
                {
                    // 플레이어가 빌딩에 맞는 아이템이 있는지 확인 후 아이템을 전송
                    if (player.HasMatchingItem(building.InventoryKey))
                    {
                        player.TransferItemToBuilding(building);  // 플레이어가 빌딩으로 아이템 전송
                    }

                    // 빌딩에서 플레이어로 아이템을 전송 (ReceiveItem 호출)
                    building.TransferItemToCreature(player);  // 빌딩에서 플레이어로 아이템 전달

                    // 인벤토리가 가득 찼거나 빌딩의 아이템이 없으면 큐에서 제거
                    if (player.InventoryIsFull())
                    {
                        queue.Dequeue();  // 인벤토리가 가득 차면 큐에서 제거
                    }
                }
            }
        }

        // 빌딩 ID 등록
        private bool RegisterInstanceID(BaseBuilding building)
        {
            return _buildings.TryAdd(building.transform.GetInstanceID(), building);
        }
    }
}