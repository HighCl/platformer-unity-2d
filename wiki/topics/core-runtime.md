---
topic: Core Runtime
type: codebase
last_compiled: 2026-05-16
source_count: 8
status: active
---

# Core Runtime

## Purpose [coverage: high -- 8 sources]
Core 런타임은 플레이어 조작, 카메라 추적, 공통 인터페이스, 애니메이터 파라미터, 입력 액션 래퍼를 담당한다. Game 어셈블리가 사용할 수 있는 기본 동작과 계약을 제공하지만 UI나 Game 구현에는 의존하지 않는다.

가장 중요한 클래스는 `PlayerController`다. 이동, 점프, 공중 점프, 낙하 사망, 리스폰 위치 적용, 사망 애니메이션, 이동 플랫폼 속도 합산까지 플레이어 중심 런타임을 처리한다.

## Architecture [coverage: high -- 8 sources]
Core 어셈블리는 `Platformer.Data`와 `Unity.InputSystem`만 참조한다.

핵심 파일:
- `PlayerController.cs`: 입력, 물리, 상태, 사망, 씬 리로드 처리
- `InputSystem_Actions.cs`: Unity Input System 생성 코드
- `IDamageable.cs`: 데미지 수신 계약
- `IMovingGround.cs`: 이동 지면 속도 제공 계약
- `AnimatorParams.cs`: 애니메이터 파라미터 문자열 상수
- `CameraFollow.cs`: 대상 Transform을 부드럽게 추적

## Talks To [coverage: medium -- 4 sources]
- **Platformer.Data**: `RespawnData`, `PlayerState`를 사용한다.
- **Unity Input System**: `Player/Move`, `Player/Jump` 액션을 읽는다.
- **Unity Physics2D**: `Rigidbody2D`, `OverlapCircle`, `OverlapCircleNonAlloc`로 이동과 지면 감지를 처리한다.
- **SceneManager**: 플레이어 사망 후 현재 씬을 다시 로드한다.

## API Surface [coverage: medium -- 4 sources]
- `IDamageable.TakeDamage(int amount)`: 적과 플레이어가 공유하는 데미지 진입점
- `IMovingGround.GetPointVelocity(Vector2 worldPoint)`: 플레이어가 올라탄 지면 속도 조회
- `PlayerController.TakeDamage(int amount)`: 플레이어 사망 트리거
- `InputSystem_Actions.Player.Move`, `InputSystem_Actions.Player.Jump`: 플레이어 입력 액션

## Data [coverage: medium -- 4 sources]
`PlayerController`는 `RespawnData`의 `hasCheckpoint`와 `position`을 시작 시 읽어 체크포인트 위치로 이동한다. 내부 상태는 `PlayerState` enum으로 관리하며, 이동 플랫폼 보정을 위해 `_groundOverlapResults` 버퍼를 사용한다.

## Key Decisions [coverage: high -- 8 sources]
- **입력은 Update, 물리는 FixedUpdate**: 입력 플래그와 이동값은 `Update`에서 읽고 Rigidbody 변경은 `FixedUpdate`에서 수행한다.
- **지면 감지는 OverlapCircle 기반**: `Raycast` 대신 발 위치의 원형 감지로 Ground 레이어를 확인한다.
- **이동 플랫폼 속도 합산**: `IMovingGround`를 통해 플랫폼 속도를 플레이어 속도에 더한다.
- **사망 시 Rigidbody 동결**: `simulated=false` 대신 속도, 중력, constraints를 조정해 애니메이터 동작을 보존한다.

## Gotchas [coverage: high -- 8 sources]
- `PlayerController`에는 `_moveSpeed`, `_jumpForce` 같은 숫자 직렬화 필드가 남아 있어, 프로젝트 규칙의 SO Settings 패턴과 완전히 일치하지 않는다.
- 좌우 반전은 현재 `transform.localScale.x`를 사용한다. 프로젝트 게임 로직 스킬은 `SpriteRenderer.flipX`를 권장하므로 GroundCheck 자식 위치 영향을 검토해야 한다.
- `InputSystem_Actions.cs`는 생성 코드라 직접 편집 대상이 아니다.
- `PlayerController`는 `Rigidbody2D`, `Animator`를 `GetComponent`로 기대하지만 `RequireComponent`가 없다.

## Sources
- [Assets/_Project/Scripts/Core/PlayerController.cs](../../Assets/_Project/Scripts/Core/PlayerController.cs)
- [Assets/_Project/Scripts/Core/InputSystem_Actions.cs](../../Assets/_Project/Scripts/Core/InputSystem_Actions.cs)
- [Assets/_Project/Scripts/Core/AnimatorParams.cs](../../Assets/_Project/Scripts/Core/AnimatorParams.cs)
- [Assets/_Project/Scripts/Core/CameraFollow.cs](../../Assets/_Project/Scripts/Core/CameraFollow.cs)
- [Assets/_Project/Scripts/Core/IDamageable.cs](../../Assets/_Project/Scripts/Core/IDamageable.cs)
- [Assets/_Project/Scripts/Core/IMovingGround.cs](../../Assets/_Project/Scripts/Core/IMovingGround.cs)
- [Assets/_Project/Scripts/Core/Platformer.Core.asmdef](../../Assets/_Project/Scripts/Core/Platformer.Core.asmdef)
- [Assets/_Project/Scripts/Data/PlayerState.cs](../../Assets/_Project/Scripts/Data/PlayerState.cs)
