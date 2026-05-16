---
topic: Agent Workflow
type: codebase
last_compiled: 2026-05-16
source_count: 13
status: active
---

# Agent Workflow

## Purpose [coverage: high -- 13 sources]
Agent Workflow는 Codex가 이 Unity 프로젝트에서 안전하게 작업하기 위한 절차와 역할 분담을 정의한다. 기존 Cursor/Claude 호환 규칙에서 Codex 전용 `.agents/skills/` 체계로 이동하는 변경이 핵심이다.

## Architecture [coverage: high -- 13 sources]
스킬은 두 층으로 나뉜다.

- **상위 가드레일**: `platformer-guardrails`, `platformer-architecture`, `platformer-coding-style`, `platformer-game-logic`, `platformer-ui`, `platformer-unity-nondev-basics`
- **작업 스킬**: `add-feature`, `apply-spritesheet`, `apply-tileset`, `replace-sprite`

README는 Codex 기준 가드레일이 `.agents/skills/platformer-*`라고 설명하며, `AGENTS.md`는 어셈블리 경계, 테스트, Unity 직렬화 파일 보호 규칙을 통합한다.

## Talks To [coverage: high -- 8 sources]
- **Unity Editor**: Refresh, Console, Test Runner로 변경사항 반영과 검증을 수행한다.
- **사용자 수동 조작**: 씬/프리팹/Inspector 조작은 단계별 안내로 처리한다.
- **Git**: 커밋 메시지는 `<type>: <설명>` 형식을 따른다.

## API Surface [coverage: medium -- 4 sources]
- `platformer-guardrails`: 작업 시작 시 최상위 스킬 선택 기준
- `platformer-architecture`: 어셈블리, namespace, Unity 직렬화 보호
- `platformer-coding-style`: C# 네이밍, 폴더, 테스트, Input System
- `add-feature`, `apply-spritesheet`, `apply-tileset`, `replace-sprite`: 기능/에셋 작업 절차

## Data [coverage: medium -- 4 sources]
스킬 문서는 Markdown frontmatter의 `name`, `description`으로 트리거 조건을 정의한다. `.agents/skills/platformer-*`는 신규 미추적 파일이며, `.claude/skills/*`는 삭제 상태다.

## Key Decisions [coverage: high -- 13 sources]
- **Codex 기준 스킬 체계 채택**: Cursor의 alwaysApply 규칙을 Codex가 자동 적용하지 못하므로 `.agents/skills`로 규칙을 이관했다.
- **수동 에디터 조작 안내**: 씬/프리팹/Inspector 변경은 파일 직접 편집 대신 사용자 안내로 처리한다.
- **작업 후 컴파일 확인**: Unity 에디터 Refresh와 Console 확인을 검증 기준으로 둔다.

## Gotchas [coverage: high -- 13 sources]
- `.agents/skills`가 아직 미추적이면 다른 환경에서 Codex 스킬이 누락될 수 있다.
- `.claude/skills` 삭제와 `.agents/skills` 추가가 함께 있어, 커밋 시 의도한 마이그레이션인지 확인해야 한다.
- 로그 확인 명령은 현재 Windows PowerShell 환경에 맞춰 실행해야 한다.

## Sources
- [AGENTS.md](../../AGENTS.md)
- [README.md](../../README.md)
- [.agents/skills/platformer-guardrails/SKILL.md](../../.agents/skills/platformer-guardrails/SKILL.md)
- [.agents/skills/platformer-architecture/SKILL.md](../../.agents/skills/platformer-architecture/SKILL.md)
- [.agents/skills/platformer-coding-style/SKILL.md](../../.agents/skills/platformer-coding-style/SKILL.md)
- [.agents/skills/platformer-game-logic/SKILL.md](../../.agents/skills/platformer-game-logic/SKILL.md)
- [.agents/skills/platformer-ui/SKILL.md](../../.agents/skills/platformer-ui/SKILL.md)
- [.agents/skills/platformer-unity-nondev-basics/SKILL.md](../../.agents/skills/platformer-unity-nondev-basics/SKILL.md)
- [.agents/skills/add-feature/SKILL.md](../../.agents/skills/add-feature/SKILL.md)
- [.agents/skills/apply-spritesheet/SKILL.md](../../.agents/skills/apply-spritesheet/SKILL.md)
- [.agents/skills/apply-tileset/SKILL.md](../../.agents/skills/apply-tileset/SKILL.md)
- [.agents/skills/replace-sprite/SKILL.md](../../.agents/skills/replace-sprite/SKILL.md)
