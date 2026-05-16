---
name: platformer-guardrails
description: Platformer Unity 2D 프로젝트에서 Codex가 코드 수정, 기능 추가, 버그 수정, 리뷰, 씬/프리팹/에셋 조작, 빌드, 테스트, 문서 작업을 수행할 때 항상 먼저 적용하는 최상위 가드레일 스킬. Cursor의 .cursor/rules alwaysApply 규칙을 Codex용 .agents/skills 흐름으로 대체한다. 이 저장소에서 작업을 시작하거나 어떤 하위 스킬을 적용해야 할지 판단할 때 사용한다.
---

# Platformer Codex 가드레일

## 기본 원칙

- 이 프로젝트는 Unity 6.0.3, URP 2D 기반 2D 플랫포머 프로젝트다.
- Codex는 `.cursor/rules`의 `alwaysApply`를 자동 적용하지 못한다.
- 따라서 이 스킬을 최상위 진입점으로 사용하고, 작업 범위에 맞는 세부 스킬을 함께 적용한다.
- 기존 `.cursor/rules`는 원본 규칙 문서로 남겨두되, 실제 Codex 작업 기준은 `.agents/skills/platformer-*` 스킬이다.

## 작업 시작 순서

1. 코드 작성과 테스트 파일 생성은 Codex가 수행한다.
2. 씬/프리팹/Inspector 조작은 Unity 직렬화 파일을 직접 편집하지 않고, 사용자에게 단계별로 안내한다.
3. 코드 수정 후 Unity 에디터에서 `Assets > Refresh` 또는 `Ctrl+R`로 반영하고 `Console`/`Test Runner`로 확인한다.

## 세부 스킬 선택

- 모든 코드/에셋/씬 작업: `platformer-architecture`, `platformer-coding-style`
- `Assets/_Project/Scripts/Core/**/*.cs`, `Assets/_Project/Scripts/Game/**/*.cs`: `platformer-game-logic`
- `Assets/_Project/Scripts/UI/**/*.cs` 또는 Canvas/HUD/메뉴/팝업: `platformer-ui`
- 비개발자에게 Unity 용어, 오류 의미, 수동 조작법을 설명할 때: `platformer-unity-nondev-basics`
- 새 게임 기능 추가: 기존 `add-feature`
- 스프라이트 시트 적용: 기존 `apply-spritesheet`
- 타일셋 적용: 기존 `apply-tileset`
- 스프라이트 교체: 기존 `replace-sprite`
- 빌드: 기존 `build`

## 절대 금지

- Game과 UI 직접 참조 금지.
- `.unity`, `.prefab`, `.asset`, `.controller`, `.anim` 파일 직접 텍스트 편집 금지.
- `Assets/_Project/Arts/` 하위에 `.cs` 파일 생성 금지.
- 레거시 `Input.GetKey`, `Input.GetAxis` 사용 금지.
- 새 기능에서 테스트 없이 완료 보고 금지.
- 씬/프리팹 조작을 임의 파일 편집으로 대체 금지.

## 검증 기준

- 코드 수정 후 Unity 에디터 Refresh로 컴파일 반영을 확인한다.
- 컴파일 오류가 있으면 해결 전까지 다음 작업으로 넘어가지 않는다.
- 새 기능 또는 동작 변경에는 대응 테스트를 추가한다.
- 가능한 경우 Unity Test Runner까지 수행한다.

## 문서 동기화

- 프로젝트 가드레일을 수정하면 `.agents/skills/platformer-*` 스킬을 우선 갱신한다.
- `.cursor/rules`는 Cursor 호환 원본으로 유지할 수 있으나, Codex 기준 문서는 `.agents/skills`다.
