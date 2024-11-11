using Interfaces;
using Units.Stages.Modules.BattleModules.Abstract;
using Units.Stages.Modules.MovementModules.Units;
using UnityEngine;

namespace Units.Stages.Modules.BattleModules.Units
{
    public interface IHunterBattleModule : IInitializable
    {
        public void Update(); // 무기를 회전시키는 메서드
        public void HandleOnTriggerHuntingZone(bool value);
    }

    public class HunterBattleModule : BattleModule, IHunterBattleModule
    {
        private readonly IHunterMovementModule _hunterMovementModule;
        private readonly Transform _hunterTransform;
        private readonly float _initialAngleOffset;
        private readonly Quaternion _initialRotation; // 무기의 초기 회전 값을 저장
        private readonly Weapon _weapon;

        private bool _isInHuntingZone;

        public HunterBattleModule(IHunterMovementModule hunterMovementModule, Transform hunterTransform, Weapon weapon)
        {
            _hunterMovementModule = hunterMovementModule;
            _hunterTransform = hunterTransform;
            _weapon = weapon;

            // 무기의 초기 회전 값과 각도 저장
            _initialRotation = _weapon.transform.localRotation;
            _initialAngleOffset = _weapon.transform.localEulerAngles.z;
        }

        public void Initialize()
        {
            _weapon.transform.localRotation = _initialRotation;
            _weapon.transform.localEulerAngles = new Vector3(_weapon.transform.localEulerAngles.x,
                _weapon.transform.localEulerAngles.y, _initialAngleOffset);
            _weapon.transform.gameObject.SetActive(false);
        }

        public void HandleOnTriggerHuntingZone(bool value)
        {
            _isInHuntingZone = value;
            ActivateWeapon(value);
        }

        public void Update()
        {
            if (!_isInHuntingZone) return;

            Vector2 direction = _hunterMovementModule.Direction;

            if (direction == Vector2.zero) return;

            // 조이스틱 방향에 따른 각도 계산
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var totalAngle = angle + _initialAngleOffset;

            // 좌우 플립 처리
            Vector3 weaponScale = _weapon.transform.localScale;

            if (direction.x < 0)
            {
                // 왼쪽 방향일 때
                weaponScale.x = -Mathf.Abs(weaponScale.x); // Flip 처리
                totalAngle += 180; // 각도 보정
            }
            else
            {
                // 오른쪽 방향일 때
                weaponScale.x = Mathf.Abs(weaponScale.x);
            }

            // 무기의 회전을 플레이어 기준으로 설정
            _weapon.transform.rotation = _hunterTransform.rotation * Quaternion.Euler(0, 0, totalAngle);

            // // 무기의 위치를 조이스틱 방향에 맞게 조정
            // _weapon.transform.localPosition = new Vector3(direction.x, direction.y, 0).normalized;

            // 플립 적용
            _weapon.transform.localScale = weaponScale;
        }

        private void ActivateWeapon(bool value)
        {
            _weapon.ActivateWeapon(value);
        }
    }
}