---
topic: UI HUD
type: codebase
last_compiled: 2026-05-16
source_count: 5
status: active
---

# UI HUD

## Purpose [coverage: medium -- 5 sources]
UI 영역은 HUD와 화면 표시 로직을 담당한다. 현재 구현은 `HUDController` 중심의 최소 구조이며, `GameEvent`를 구독해 플레이어 사망 같은 게임 이벤트를 UI에서 처리할 수 있는 기반만 마련되어 있다.

## Architecture [coverage: medium -- 5 sources]
`Platformer.UI.asmdef`는 `Platformer.Data`, `Platformer.Core`를 참조한다. `Platformer.Game`은 참조하지 않는다.

핵심 파일:
- `HUDController.cs`: `_onPlayerDied` 이벤트 구독과 해제
- `HUDControllerTests.cs`: 생성 및 enable/disable 안정성 테스트
- `platformer-ui` 스킬: Game 네임스페이스 참조 금지, TMP 사용, Canvas 계층 규칙

## Talks To [coverage: medium -- 4 sources]
- **Platformer.Data**: `GameEvent`를 구독한다.
- **Platformer.Core**: 어셈블리 참조는 허용되어 있으나 현재 `HUDController` 직접 사용은 없다.
- **Unity Canvas/TMP**: 스킬 문서상 UI 텍스트는 TextMeshPro를 사용해야 한다.

## API Surface [coverage: low -- 1 source]
현재 공개 API는 없다. `HUDController`는 Unity 라이프사이클에서 이벤트를 구독하고 `_HandlePlayerDied()` private 핸들러를 호출하는 구조다.

## Data [coverage: low -- 1 source]
`HUDController`는 `_onPlayerDied` 직렬화 필드만 가진다. Inspector에서 `GameEvent` 인스턴스를 연결해야 한다.

## Key Decisions [coverage: medium -- 4 sources]
- **UI는 Game을 모른다**: UI는 Game 네임스페이스를 참조하지 않고 SO 이벤트를 구독한다.
- **구독/해제 대칭**: `OnEnable`에서 구독하고 `OnDisable`에서 해제한다.
- **Canvas 구조 규칙화**: HUD, Menus, Popups 그룹을 나누는 계층이 권장된다.

## Gotchas [coverage: medium -- 4 sources]
- `_HandlePlayerDied()`는 현재 비어 있어 실제 HUD 반응은 아직 구현되지 않았다.
- `HUDController`는 null 검사를 수행하므로 이벤트 미연결 상태에서도 enable/disable 테스트는 통과할 수 있다.
- UI가 게임 상태를 직접 조회하도록 확장하면 어셈블리 규칙을 깨기 쉽다.

## Sources
- [Assets/_Project/Scripts/UI/HUDController.cs](../../Assets/_Project/Scripts/UI/HUDController.cs)
- [Assets/_Project/Scripts/UI/Platformer.UI.asmdef](../../Assets/_Project/Scripts/UI/Platformer.UI.asmdef)
- [Assets/_Project/Scripts/UI/Tests/HUDControllerTests.cs](../../Assets/_Project/Scripts/UI/Tests/HUDControllerTests.cs)
- [Assets/_Project/Scripts/UI/Tests/Platformer.UI.Tests.asmdef](../../Assets/_Project/Scripts/UI/Tests/Platformer.UI.Tests.asmdef)
- [.agents/skills/platformer-ui/SKILL.md](../../.agents/skills/platformer-ui/SKILL.md)
