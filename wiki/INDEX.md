# Platformer Unity 2D Knowledge Base

Last compiled: 2026-05-16
Total topics: 8 | Total sources: 52

## Topics

| Topic | Also Known As | Sources | Last Updated | Status |
|-------|--------------|---------|-------------|--------|
| [project-overview](topics/project-overview.md) | 프로젝트 개요, 구조, README, AGENTS | 7 | 2026-05-16 | active |
| [core-runtime](topics/core-runtime.md) | Core, PlayerController, InputSystem, 카메라 | 8 | 2026-05-16 | active |
| [data-events-state](topics/data-events-state.md) | Data, GameEvent, RespawnData, PlayerState, SO | 10 | 2026-05-16 | active |
| [gameplay-systems](topics/gameplay-systems.md) | Game, Enemy, MovingPlatform, Checkpoint, FinishFlag | 10 | 2026-05-16 | active |
| [ui-hud](topics/ui-hud.md) | UI, HUDController, Canvas, GameEvent 구독 | 5 | 2026-05-16 | active |
| [testing](topics/testing.md) | Tests, PlayMode, NUnit, Test Runner | 16 | 2026-05-16 | active |
| [agent-workflow](topics/agent-workflow.md) | Codex skills, .agents, guardrails | 13 | 2026-05-16 | active |
| [unity-project-configuration](topics/unity-project-configuration.md) | Unity version, packages, asmdef, manifest | 8 | 2026-05-16 | active |

## Concepts

| Concept | Connects | Last Updated |
|---------|----------|-------------|
| [guardrail-first-development](concepts/guardrail-first-development.md) | project-overview, agent-workflow, testing, unity-project-configuration | 2026-05-16 |

## Recent Changes

- 2026-05-16: 첫 위키 설정과 8개 토픽, 1개 컨셉을 생성했다.
- 2026-05-16: `RespawnDataTests.cs`의 의도적 실패 테스트와 신규 Core PlayMode 테스트를 `testing` 토픽에 반영했다.
- 2026-05-16: `ProjectVersion.txt`의 Unity 버전 변경(`6000.3.11f1` -> `6000.3.9f1`)을 `unity-project-configuration` 토픽에 반영했다.
- 2026-05-16: Unity 직렬화 변경 파일(`.prefab`, `.asset`)은 직접 해석하지 않고 Gotchas에만 반영했다.
