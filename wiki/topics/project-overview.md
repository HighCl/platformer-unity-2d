---
topic: Project Overview
type: codebase
last_compiled: 2026-05-16
source_count: 7
status: active
---

# Project Overview

## Purpose [coverage: high -- 7 sources]
이 프로젝트는 Unity 6.0.3 계열, URP 2D, Input System 기반의 2D 플랫포머 데모 환경이다. 핵심 목적은 Unity에 익숙하지 않은 아트 디자이너가 Codex와 함께 스프라이트, 타일셋, 기능 추가, 테스트, 빌드까지 진행할 수 있도록 가드레일이 포함된 작업 환경을 제공하는 것이다.

프로젝트는 `Assets/_Project/` 아래에 런타임 코드, 에셋, 데이터, 프리팹, 씬을 분리하고, 외부 에셋팩은 `Assets/` 바로 아래에 둔다. Codex 기준 작업 규칙은 `.agents/skills/`와 `AGENTS.md`가 담당하며, 기존 `.cursor/rules/`는 호환용 원본 규칙으로 남아 있다.

## Architecture [coverage: high -- 7 sources]
런타임 코드는 네 개 어셈블리로 분리된다.

- `Platformer.Data`: ScriptableObject 데이터, 이벤트, enum
- `Platformer.Core`: 플레이어, 카메라, 공통 인터페이스, 입력 래퍼
- `Platformer.Game`: 체크포인트, 적, 피니시 플래그, 이동 플랫폼, 게임 매니저
- `Platformer.UI`: HUD와 UI 이벤트 구독

참조 방향은 `Data <- Core <- Game` 및 `Data/Core <- UI` 형태다. `Game`과 `UI`는 직접 참조하지 않고 ScriptableObject 이벤트 채널로만 통신한다.

## Talks To [coverage: medium -- 4 sources]
- **Unity Editor**: `Assets > Refresh`, `Console`, `Test Runner`로 반영과 검증을 확인한다.
- **Unity Input System**: `PlayerController`는 생성된 `InputSystem_Actions` 래퍼로 `Move`, `Jump` 액션을 읽는다.
- **URP 2D/2D 패키지**: 스프라이트, 타일맵, 2D Animation, SpriteShape, Tilemap Extras가 프로젝트 전제다.
- **Codex 스킬**: 작업 유형별 세부 절차는 `.agents/skills/`에 저장된다.

## API Surface [coverage: medium -- 4 sources]
프로젝트 레벨의 외부 진입점은 명령과 문서 규칙 중심이다.

- `Assets > Refresh` 또는 `Ctrl+R`: Unity 에디터 변경사항 반영
- `Window > General > Test Runner`: Unity 테스트 실행
- `.agents/skills/*/SKILL.md`: Codex 작업 절차
- `AGENTS.md`: 어셈블리, 코딩, 검증, 커밋 규칙

## Data [coverage: medium -- 4 sources]
프로젝트 데이터는 `Assets/_Project/Datas/`에 저장되는 ScriptableObject 인스턴스와 `Assets/_Project/Scripts/Data/`의 타입 정의로 구성된다. 에셋 자체는 Unity 직렬화 파일이므로 직접 텍스트 편집 대상이 아니다.

## Key Decisions [coverage: high -- 7 sources]
- **어셈블리 경계 우선**: Game/UI 순환 참조 방지를 위해 어셈블리 참조 방향을 명시했다.
- **SO 이벤트 채널 사용**: Game과 UI의 직접 참조를 금지하고 `GameEvent`로 통신한다.
- **Codex 스킬 전환**: Cursor 규칙 자동 적용 한계를 보완하기 위해 `.agents/skills/platformer-*`가 Codex 기준 규칙으로 추가되었다.
- **Unity YAML 보호**: `.unity`, `.prefab`, `.asset`, `.controller`, `.anim`은 텍스트 직접 편집 대신 Unity 에디터에서 조작한다.

## Gotchas [coverage: high -- 7 sources]
- `Assets/_Project/Prefabs/Player.prefab` 같은 Unity 직렬화 파일은 변경되어 있어도 위키 컴파일에서 직접 열어 해석하지 않는다.
- `CLAUDE.md`는 삭제 상태이고 `AGENTS.md`가 신규 기준 문서로 존재한다.
- README는 Unity 6.0.3이라고 설명하지만 `ProjectSettings/ProjectVersion.txt`는 현재 `6000.3.9f1`을 가리킨다.
- `.agents/skills/`는 신규 미추적 파일이므로 커밋 전 누락 여부를 확인해야 한다.

## Sources
- [README.md](../../README.md)
- [AGENTS.md](../../AGENTS.md)
- [Packages/manifest.json](../../Packages/manifest.json)
- [ProjectSettings/ProjectVersion.txt](../../ProjectSettings/ProjectVersion.txt)
- [Assets/_Project/Scripts/Data/Platformer.Data.asmdef](../../Assets/_Project/Scripts/Data/Platformer.Data.asmdef)
- [Assets/_Project/Scripts/Core/Platformer.Core.asmdef](../../Assets/_Project/Scripts/Core/Platformer.Core.asmdef)
- [Assets/_Project/Scripts/Game/Platformer.Game.asmdef](../../Assets/_Project/Scripts/Game/Platformer.Game.asmdef)
