---
topic: Testing
type: codebase
last_compiled: 2026-05-16
source_count: 16
status: active
---

# Testing

## Purpose [coverage: high -- 16 sources]
테스트 영역은 각 어셈블리별 Editor 테스트와 신규 Core PlayMode 테스트를 포함한다. 프로젝트 규칙상 새 기능에는 대응 테스트가 필요하며, Unity Test Runner에서 실행하는 흐름이 표준 검증 경로다.

현재 변경분 중 가장 중요한 항목은 `RespawnDataTests.cs`에 의도적 실패 테스트가 추가된 점과 `Core/PlayModeTests`에 `PlayerController` 이동 검증 테스트가 새로 생긴 점이다.

## Architecture [coverage: high -- 16 sources]
테스트는 어셈블리별 `Tests/` 폴더에 있고, 신규 PlayMode 테스트는 `Assets/_Project/Scripts/Core/PlayModeTests/`에 분리되어 있다.

테스트 어셈블리:
- `Platformer.Data.Tests`: Data 타입 단위 테스트
- `Platformer.Core.Tests`: Core 관련 Editor 테스트
- `Platformer.Core.PlayModeTests`: Main 씬 로드 후 입력 기반 이동 검증
- `Platformer.Game.Tests`: 게임 컴포넌트와 프리팹 연결 테스트
- `Platformer.UI.Tests`: HUD 생성과 enable/disable 테스트

## Talks To [coverage: high -- 8 sources]
- **NUnit**: 모든 단위 테스트는 `[Test]`를 사용한다.
- **Unity Test Framework**: PlayMode 테스트는 `[UnityTest]`와 코루틴 기반 검증을 사용한다.
- **InputSystem TestFramework**: 신규 PlayMode 테스트가 `InputTestFixture`와 가상 `Keyboard`를 사용한다.
- **UnityEditor**: 일부 Game 테스트는 `AssetDatabase`, `SerializedObject`로 프리팹 연결을 검증한다.

## API Surface [coverage: medium -- 4 sources]
- Unity 메뉴 `Window > General > Test Runner`: 프로젝트 테스트 실행 진입점
- `PlayerControllerPlayModeTests.MainScene_Player_PressDForOneSecond_IncreasesXByExpectedDistanceWithinTenPercent()`: Main 씬에서 D 키 1초 입력 후 x축 이동량 검증
- `RespawnDataTests.SetCheckpoint_IntentionalFailure_ForTestRunnerCheck()`: 테스트 러너 확인용 의도적 실패 케이스

## Data [coverage: medium -- 4 sources]
PlayMode 테스트는 `Main` 씬 이름, 기대 이동량 `5f`, 이동 시간 `1f`, 허용 오차 `10%`를 상수로 둔다. Data 테스트는 `ScriptableObject.CreateInstance<T>()`로 테스트 전용 SO를 생성하고 `DestroyImmediate`로 정리한다.

## Key Decisions [coverage: high -- 16 sources]
- **테스트 어셈블리 분리**: 런타임 어셈블리와 테스트 어셈블리를 `.asmdef`로 분리한다.
- **Editor 테스트와 PlayMode 테스트 분리**: 씬 로드와 입력 시뮬레이션이 필요한 검증은 PlayMode 전용 어셈블리로 이동했다.
- **Unity 직렬화 직접 접근 대신 UnityEditor API**: 프리팹 검증은 YAML 파싱이 아니라 `AssetDatabase.LoadAssetAtPath`를 사용한다.

## Gotchas [coverage: high -- 16 sources]
- 현재 `RespawnDataTests.SetCheckpoint_IntentionalFailure_ForTestRunnerCheck`는 `position`이 `Vector2.zero`라고 기대하므로 실제 구현과 충돌해 실패한다.
- PlayMode 테스트는 `Main` 씬이 Build Settings 또는 Test Runner 환경에서 로드 가능해야 한다.
- `PlayerControllerPlayModeTests`는 Player 태그와 `PlayerController`가 있는 오브젝트를 전제로 한다.
- `ProjectSettings/EditorBuildSettings.asset`가 변경되어 있지만 Unity 직렬화 파일이라 위키 컴파일에서는 직접 읽지 않았다.

## Sources
- [AGENTS.md](../../AGENTS.md)
- [README.md](../../README.md)
- [Assets/_Project/Scripts/Core/PlayModeTests/Platformer.Core.PlayModeTests.asmdef](../../Assets/_Project/Scripts/Core/PlayModeTests/Platformer.Core.PlayModeTests.asmdef)
- [Assets/_Project/Scripts/Core/PlayModeTests/PlayerControllerPlayModeTests.cs](../../Assets/_Project/Scripts/Core/PlayModeTests/PlayerControllerPlayModeTests.cs)
- [Assets/_Project/Scripts/Core/Tests/Platformer.Core.Tests.asmdef](../../Assets/_Project/Scripts/Core/Tests/Platformer.Core.Tests.asmdef)
- [Assets/_Project/Scripts/Core/Tests/PlayerStateTests.cs](../../Assets/_Project/Scripts/Core/Tests/PlayerStateTests.cs)
- [Assets/_Project/Scripts/Data/Tests/GameEventTests.cs](../../Assets/_Project/Scripts/Data/Tests/GameEventTests.cs)
- [Assets/_Project/Scripts/Data/Tests/MovingPlatformSettingsTests.cs](../../Assets/_Project/Scripts/Data/Tests/MovingPlatformSettingsTests.cs)
- [Assets/_Project/Scripts/Data/Tests/Platformer.Data.Tests.asmdef](../../Assets/_Project/Scripts/Data/Tests/Platformer.Data.Tests.asmdef)
- [Assets/_Project/Scripts/Data/Tests/RespawnDataTests.cs](../../Assets/_Project/Scripts/Data/Tests/RespawnDataTests.cs)
- [Assets/_Project/Scripts/Game/Tests/EnemyMeleeChaserTests.cs](../../Assets/_Project/Scripts/Game/Tests/EnemyMeleeChaserTests.cs)
- [Assets/_Project/Scripts/Game/Tests/GameManagerTests.cs](../../Assets/_Project/Scripts/Game/Tests/GameManagerTests.cs)
- [Assets/_Project/Scripts/Game/Tests/MovingPlatformTests.cs](../../Assets/_Project/Scripts/Game/Tests/MovingPlatformTests.cs)
- [Assets/_Project/Scripts/Game/Tests/Platformer.Game.Tests.asmdef](../../Assets/_Project/Scripts/Game/Tests/Platformer.Game.Tests.asmdef)
- [Assets/_Project/Scripts/UI/Tests/HUDControllerTests.cs](../../Assets/_Project/Scripts/UI/Tests/HUDControllerTests.cs)
- [Assets/_Project/Scripts/UI/Tests/Platformer.UI.Tests.asmdef](../../Assets/_Project/Scripts/UI/Tests/Platformer.UI.Tests.asmdef)
