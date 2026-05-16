# Codebase Wiki - Navigation Guide

이 프로젝트에는 컴파일된 코드베이스 위키가 있다. 전체 파일을 처음부터 훑기 전에 이 문서를 진입점으로 사용한다.

## How to use this wiki

1. [INDEX.md](INDEX.md)에서 관련 토픽을 고른다.
2. 현재 작업과 관련된 토픽 문서 1-3개를 먼저 읽는다.
3. Coverage 태그를 확인한다.
   - `[coverage: high]`: 개요 판단에 충분하다.
   - `[coverage: medium]`: 대부분의 방향은 잡을 수 있으나 세부 구현은 원본 확인이 필요하다.
   - `[coverage: low]`: 원본 파일을 직접 읽는다.
4. 여러 토픽에 걸친 판단은 [concepts/](concepts/)를 확인한다.
5. 새 코드를 작성하거나 디버깅할 때는 위키만 믿지 말고 실제 소스 파일을 확인한다.

## When NOT to use the wiki

- 정확한 타입, 메서드 시그니처, 직렬화 필드명을 확인할 때
- 특정 테스트 실패 원인을 디버깅할 때
- Unity 씬, 프리팹, 애니메이터, SO 에셋의 실제 참조 상태를 확인할 때
- 위키 문서가 `[coverage: low]`로 표시한 영역을 수정할 때

## High-priority starting points

- 플레이어 이동/사망/입력: [topics/core-runtime.md](topics/core-runtime.md)
- RespawnData, GameEvent, SO 상태: [topics/data-events-state.md](topics/data-events-state.md)
- 적, 체크포인트, 이동 플랫폼: [topics/gameplay-systems.md](topics/gameplay-systems.md)
- 현재 테스트 상태와 실패 위험: [topics/testing.md](topics/testing.md)
- Codex 작업 규칙: [topics/agent-workflow.md](topics/agent-workflow.md)

## Stats

Compiled: 2026-05-16 | Topics: 8 | Sources: 52 | Auto-updates: prompt
