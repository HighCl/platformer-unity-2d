---
topic: Gameplay Systems
type: codebase
last_compiled: 2026-05-16
source_count: 10
status: active
---

# Gameplay Systems

## Purpose [coverage: high -- 10 sources]
Gameplay 영역은 씬 안에서 동작하는 게임 규칙을 담당한다. 체크포인트, 피니시 플래그, 적 추적과 접촉 피해, 스톰프 판정, 이동 플랫폼, 전역 게임 매니저가 여기에 포함된다.

## Architecture [coverage: high -- 10 sources]
`Platformer.Game.asmdef`는 `Platformer.Data`, `Platformer.Core`를 참조한다. UI는 참조하지 않는다.

핵심 파일:
- `Checkpoint.cs`: 플레이어 트리거 진입 시 `RespawnData`에 위치 저장
- `FinishFlag.cs`: 레벨 클리어 시 체크포인트 초기화 후 현재 씬 리로드
- `EnemyMeleeChaser.cs`: 감지 반경 안의 타겟 추적, 접촉 피해, 스톰프 사망
- `EnemyStompZone.cs`: 전용 트리거로 스톰프 판정 위임
- `MovingPlatform.cs`: 키네마틱 Rigidbody2D 왕복 이동과 지면 속도 제공
- `GameManager.cs`: 단일 인스턴스 패턴의 기본 매니저

## Talks To [coverage: high -- 7 sources]
- **Core**: `IDamageable`, `IMovingGround`, `PlayerController`, `AnimatorParams`를 사용한다.
- **Data**: `RespawnData`, `MovingPlatformSettings`를 사용한다.
- **Unity Physics2D**: `Rigidbody2D`, `Collider2D`, `Collision2D`, `TilemapCollider2D` 테스트와 연결된다.
- **SceneManager**: `FinishFlag`가 현재 씬을 다시 로드한다.

## API Surface [coverage: medium -- 4 sources]
- `EnemyMeleeChaser.TakeDamage(int amount)`: 적 체력 감소와 사망
- `EnemyMeleeChaser.TryStompByPlayerCollider(Collider2D playerCollider)`: 트리거 기반 스톰프 판정
- `MovingPlatform.GetPointVelocity(Vector2 worldPoint)`: 플레이어 이동 보정을 위한 플랫폼 속도 제공
- `GameManager.Instance`: 정적 싱글톤 접근자

## Data [coverage: medium -- 4 sources]
`Checkpoint`와 `FinishFlag`는 `RespawnData`를 공유한다. `MovingPlatform`은 `MovingPlatformSettings`를 통해 속도, 끝점 대기 시간, 로컬 이동 방향, 이동 거리를 결정한다.

## Key Decisions [coverage: high -- 10 sources]
- **Game/UI 직접 참조 금지 유지**: Game 어셈블리는 UI 어셈블리를 참조하지 않는다.
- **스톰프 판정 이중화**: 충돌 노멀 기반 판정과 전용 `EnemyStompZone` 트리거를 모두 지원한다.
- **이동 플랫폼 속도 인터페이스화**: `IMovingGround` 계약으로 Core의 플레이어 이동 보정과 Game의 플랫폼 구현을 분리한다.
- **에디터/플레이 모드 Destroy 분기**: 적 사망 시 `Application.isPlaying`에 따라 `Destroy`와 `DestroyImmediate`를 구분한다.

## Gotchas [coverage: high -- 10 sources]
- `EnemyMeleeChaserTests.TakeDamage_OneHitDestroysEnemy`는 즉시 파괴 동작을 가정하므로 Unity 오브젝트 라이프사이클 차이에 민감하다.
- `MovingPlatform`은 `_settings`가 null이면 정지한다. 프리팹에 설정 SO가 연결되어야 한다.
- `FinishFlag`는 에디터에서만 `Debug.Log`를 출력한다.
- `GameManager`는 아직 빈 라이프사이클 훅만 가진 최소 구현이다.

## Sources
- [Assets/_Project/Scripts/Game/Checkpoint.cs](../../Assets/_Project/Scripts/Game/Checkpoint.cs)
- [Assets/_Project/Scripts/Game/EnemyMeleeChaser.cs](../../Assets/_Project/Scripts/Game/EnemyMeleeChaser.cs)
- [Assets/_Project/Scripts/Game/EnemyStompZone.cs](../../Assets/_Project/Scripts/Game/EnemyStompZone.cs)
- [Assets/_Project/Scripts/Game/FinishFlag.cs](../../Assets/_Project/Scripts/Game/FinishFlag.cs)
- [Assets/_Project/Scripts/Game/GameManager.cs](../../Assets/_Project/Scripts/Game/GameManager.cs)
- [Assets/_Project/Scripts/Game/MovingPlatform.cs](../../Assets/_Project/Scripts/Game/MovingPlatform.cs)
- [Assets/_Project/Scripts/Game/Platformer.Game.asmdef](../../Assets/_Project/Scripts/Game/Platformer.Game.asmdef)
- [Assets/_Project/Scripts/Game/Tests/EnemyMeleeChaserTests.cs](../../Assets/_Project/Scripts/Game/Tests/EnemyMeleeChaserTests.cs)
- [Assets/_Project/Scripts/Game/Tests/GameManagerTests.cs](../../Assets/_Project/Scripts/Game/Tests/GameManagerTests.cs)
- [Assets/_Project/Scripts/Game/Tests/MovingPlatformTests.cs](../../Assets/_Project/Scripts/Game/Tests/MovingPlatformTests.cs)
