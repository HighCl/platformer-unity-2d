# Wiki Schema

이 파일은 Platformer Unity 2D 코드베이스 위키의 구조와 명명 규칙을 정의한다. 다음 컴파일부터 토픽 분류와 새 문서 생성 기준으로 사용한다.

## Topics

- `project-overview`: 프로젝트 목적, 어셈블리 구조, 폴더 구조, Codex 작업 기준
- `core-runtime`: 플레이어, 입력, 카메라, 공통 인터페이스, Core 어셈블리
- `data-events-state`: ScriptableObject 데이터, 이벤트 채널, 상태 enum, Data 테스트
- `gameplay-systems`: 체크포인트, 피니시 플래그, 적, 이동 플랫폼, Game 어셈블리
- `ui-hud`: HUD, UI 이벤트 구독, UI 어셈블리 규칙
- `testing`: Editor/PlayMode 테스트, 테스트 어셈블리, 현재 실패 위험
- `agent-workflow`: `.agents/skills` 기반 Codex 작업 절차와 Unity 에디터 수동 조작 안내
- `unity-project-configuration`: Unity 버전, 패키지, `.asmdef`, 테스트 러너 설정

## Concepts

- `guardrail-first-development`: 구현 전 가드레일, 테스트, 어셈블리 경계를 우선하는 개발 패턴 - connects [project-overview, agent-workflow, testing, unity-project-configuration]

## Article Structure

각 토픽 문서는 아래 형식을 따른다.

- **Purpose** [coverage] -- 해당 모듈/영역이 맡는 역할
- **Architecture** [coverage] -- 핵심 파일, 클래스, 어셈블리 구조
- **Talks To** [coverage] -- 다른 모듈, Unity 시스템, 외부 도구와의 연결
- **API Surface** [coverage] -- public 메서드, 인터페이스, 테스트 진입점
- **Data** [coverage] -- ScriptableObject, 런타임 상태, 설정값
- **Key Decisions** [coverage] -- 구조적 결정과 이유
- **Gotchas** [coverage] -- 주의점, 실패 위험, 변경 시 확인할 사항
- **Sources** -- 모든 기여 원본 파일 링크

Coverage 태그는 `[coverage: high -- N sources]`, `[coverage: medium -- N sources]`, `[coverage: low -- N sources]` 형식을 사용한다.

## Naming Conventions

- 토픽 슬러그: lowercase-kebab-case
- 토픽 파일: `wiki/topics/{topic-slug}.md`
- 컨셉 파일: `wiki/concepts/{concept-slug}.md`
- 날짜: `YYYY-MM-DD`
- 링크: Markdown 상대 경로

## Cross-Reference Rules

- Game/UI 경계, 테스트, Unity 직렬화 보호처럼 여러 토픽에 걸친 규칙은 관련 토픽의 Gotchas 또는 Key Decisions에 반복 기재한다.
- Unity 직렬화 에셋은 원본 링크로만 언급하고 직접 해석하지 않는다.
- 변경 소스가 테스트 결과에 직접 영향을 주면 `testing` 토픽에 반드시 반영한다.

## Evolution Log

- 2026-05-16: Initial schema generated from 8 topics and 1 concept.
