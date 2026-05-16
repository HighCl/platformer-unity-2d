---
topic: Data Events State
type: codebase
last_compiled: 2026-05-16
source_count: 10
status: active
---

# Data Events State

## Purpose [coverage: high -- 10 sources]
Data 영역은 다른 어셈블리에서 공유하는 순수 데이터와 ScriptableObject 기반 런타임 상태, 이벤트 채널, 설정을 담당한다. 이 어셈블리는 Core, Game, UI를 참조하지 않으므로 프로젝트의 의존성 루트 역할을 한다.

## Architecture [coverage: high -- 10 sources]
`Platformer.Data.asmdef`는 참조가 없는 독립 어셈블리다.

핵심 파일:
- `GameEvent.cs`: ScriptableObject 이벤트 채널
- `GameEventListener.cs`: Inspector UnityEvent 어댑터
- `RespawnData.cs`: 체크포인트 존재 여부와 위치 저장
- `MovingPlatformSettings.cs`: 이동 플랫폼 속도, 대기 시간, 방향, 거리 설정
- `PlayerState.cs`: 플레이어 상태 enum

## Talks To [coverage: medium -- 4 sources]
- **Core**: `PlayerController`가 `RespawnData`와 `PlayerState`를 사용한다.
- **Game**: `Checkpoint`, `FinishFlag`, `MovingPlatform`이 Data 타입을 사용한다.
- **UI**: `HUDController`가 `GameEvent`를 구독한다.
- **Unity Inspector**: ScriptableObject와 `GameEventListener`는 Inspector 연결을 전제로 한다.

## API Surface [coverage: high -- 7 sources]
- `GameEvent.Raise()`: 등록된 리스너 실행
- `GameEvent.AddListener(UnityAction listener)`: 이벤트 구독
- `GameEvent.RemoveListener(UnityAction listener)`: 이벤트 구독 해제
- `RespawnData.SetCheckpoint(Vector2 pos)`: 체크포인트 활성화와 위치 저장
- `RespawnData.Reset()`: 체크포인트 상태 초기화

## Data [coverage: high -- 7 sources]
`RespawnData`는 `hasCheckpoint`와 `position`을 public 필드로 가진다. `MovingPlatformSettings`는 `moveSpeed`, `pauseAtEndpoints`, `localTravelDirection`, `travelDistance`를 public 필드로 가진다. 두 타입 모두 ScriptableObject로 생성되며 SO 인스턴스는 `Assets/_Project/Datas/`에 둔다.

## Key Decisions [coverage: high -- 10 sources]
- **Data 무참조 원칙**: Game/UI/Core 순환 참조를 막기 위해 Data는 어떤 프로젝트 어셈블리도 참조하지 않는다.
- **SO 이벤트 채널**: UI와 Game의 직접 참조를 피하기 위해 `GameEvent`를 사용한다.
- **런타임 체크포인트 상태 분리**: 씬 오브젝트가 아니라 `RespawnData` SO가 마지막 체크포인트를 보관한다.

## Gotchas [coverage: high -- 10 sources]
- `GameEventListener.OnEnable()`은 `_event`와 `_response` null 검사를 하지 않는다. Inspector 미연결 시 `NullReferenceException`이 날 수 있다.
- `RespawnDataTests.cs`에는 현재 `SetCheckpoint_IntentionalFailure_ForTestRunnerCheck` 테스트가 추가되어 의도적으로 실패하도록 작성되어 있다.
- SO `.asset` 값 수정은 `assets-modify` 부분 수정 시 필드 리셋 위험이 있으므로 전체 지정 또는 안전한 방식이 필요하다.

## Sources
- [Assets/_Project/Scripts/Data/GameEvent.cs](../../Assets/_Project/Scripts/Data/GameEvent.cs)
- [Assets/_Project/Scripts/Data/GameEventListener.cs](../../Assets/_Project/Scripts/Data/GameEventListener.cs)
- [Assets/_Project/Scripts/Data/MovingPlatformSettings.cs](../../Assets/_Project/Scripts/Data/MovingPlatformSettings.cs)
- [Assets/_Project/Scripts/Data/Platformer.Data.asmdef](../../Assets/_Project/Scripts/Data/Platformer.Data.asmdef)
- [Assets/_Project/Scripts/Data/PlayerState.cs](../../Assets/_Project/Scripts/Data/PlayerState.cs)
- [Assets/_Project/Scripts/Data/RespawnData.cs](../../Assets/_Project/Scripts/Data/RespawnData.cs)
- [Assets/_Project/Scripts/Data/Tests/GameEventTests.cs](../../Assets/_Project/Scripts/Data/Tests/GameEventTests.cs)
- [Assets/_Project/Scripts/Data/Tests/MovingPlatformSettingsTests.cs](../../Assets/_Project/Scripts/Data/Tests/MovingPlatformSettingsTests.cs)
- [Assets/_Project/Scripts/Data/Tests/Platformer.Data.Tests.asmdef](../../Assets/_Project/Scripts/Data/Tests/Platformer.Data.Tests.asmdef)
- [Assets/_Project/Scripts/Data/Tests/RespawnDataTests.cs](../../Assets/_Project/Scripts/Data/Tests/RespawnDataTests.cs)
