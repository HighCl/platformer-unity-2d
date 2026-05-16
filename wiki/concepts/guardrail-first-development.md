---
concept: Guardrail First Development
last_compiled: 2026-05-16
topics_connected: [project-overview, agent-workflow, testing, unity-project-configuration]
status: active
---

# Guardrail First Development

## Pattern
이 프로젝트는 기능 구현보다 작업 가드레일을 먼저 세우는 방향으로 설계되어 있다. 어셈블리 경계, Unity 직렬화 파일 보호, Unity 에디터 검증, 테스트 작성, 스킬 기반 작업 절차가 코드와 문서 전반에 반복된다.

이 패턴의 목적은 비개발자가 AI와 함께 Unity 프로젝트를 다룰 때 발생하기 쉬운 참조 순환, 프리팹 손상, 씬 직접 편집, 테스트 누락을 구조적으로 줄이는 것이다.

## Instances
- **2026-05-16** in [project-overview](../topics/project-overview.md): `AGENTS.md`와 README가 Codex 스킬 기반 작업 흐름을 프로젝트 핵심 구조로 둔다.
- **2026-05-16** in [agent-workflow](../topics/agent-workflow.md): `.agents/skills/platformer-*`가 세부 스킬 선택과 Unity 에디터 검증 절차를 정의한다.
- **2026-05-16** in [testing](../topics/testing.md): 새 PlayMode 테스트와 어셈블리별 테스트 구조가 검증을 구현 과정의 일부로 둔다.
- **2026-05-16** in [unity-project-configuration](../topics/unity-project-configuration.md): `.asmdef`와 패키지 설정이 코드 경계와 테스트 러너를 명시적으로 고정한다.

## What This Means
이 코드베이스를 다룰 때는 "빠른 구현"보다 "작업 경계 확인 → 컴파일 확인 → 코드 수정 → 테스트" 흐름을 우선해야 한다. 특히 Unity 직렬화 파일이 변경되어 있어도 텍스트로 분석하거나 편집하지 않고, Unity 에디터에서 조작해야 한다.

## Sources
- [project-overview](../topics/project-overview.md)
- [agent-workflow](../topics/agent-workflow.md)
- [testing](../topics/testing.md)
- [unity-project-configuration](../topics/unity-project-configuration.md)
