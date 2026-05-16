# platformer-unity-2d

유니티를 모르는 아트 디자이너가 AI(Codex)와 함께 2D 플랫포머 게임을 만들 수 있도록 설계된 가드레일 환경.

## 배경

비개발자가 AI 도구를 활용해 에셋스토어 데모 수준의 게임을 직접 만들어보는 것이 목표.

## 프로젝트 스택

- Unity 6.0.3 (URP 2D)
- Input System
- 2D Animation / Tilemap

## 가드레일 구조

BPE에서 검증된 3단 방어 패턴을 Unity에 적용.

### 1단계 — Codex 스킬 규칙 (.agents/skills/)

| 규칙 | 적용 | 역할 |
|---|---|---|
| platformer-guardrails | 프로젝트 작업 시작 시 | Codex 최상위 가드레일, 세부 스킬 선택 |
| platformer-architecture | 코드/씬/프리팹/에셋 작업 | 어셈블리 경계, Game/UI 분리, Unity 직렬화 보호 |
| platformer-coding-style | 코드 작성/수정 | 네이밍, 라이프사이클, Input System, 컴파일 확인 |
| platformer-game-logic | Game/, Core/ | 물리 패턴, 콜라이더, 타일맵, 입력 처리 |
| platformer-ui | UI/ | Canvas 규칙, 이벤트 구독, Game 참조 금지 |
| platformer-unity-nondev-basics | 요청 시 | Unity 용어 쉬운 설명, 자주 생기는 문제/해결 |

### 2단계 — 작업 스킬 (.agents/skills/)

| 스킬 | 용도 |
|---|---|
| apply-spritesheet | 캐릭터 시트 PNG → 슬라이싱 → 애니메이션 → 씬 적용 |
| apply-tileset | 타일셋 시트 PNG → Tile 에셋 → 타일맵 배치 |
| replace-sprite | 기존 스프라이트 단순 교체 |
| add-feature | 새 게임 기능 추가 절차 |
| build | 프로젝트 빌드 절차 |

### 3단계 — 검증

- Unity 에디터에서 `Assets > Refresh` 또는 `Ctrl+R`로 변경사항 반영
- `Console` 창에서 컴파일 오류 0개 확인
- `Window > General > Test Runner`에서 `Run All` 실행

## 어셈블리 구조

```
Platformer.Data  ←  Platformer.Core  ←  Platformer.Game
                 ←  Platformer.UI
```

- Data: 순수 데이터 (enum, SO 이벤트, 설정)
- Core: 캐릭터, 물리, 인터페이스
- Game: 게임 매니저, 픽업, 트랩
- UI: HUD, 메뉴 (Game 참조 금지, SO 이벤트로 소통)

단방향 참조. Game ↔ UI 직접 참조 불가.

## 폴더 구조

```
Assets/_Project/
├── Scripts/
│   ├── Data/        (Platformer.Data.asmdef)
│   ├── Core/        (Platformer.Core.asmdef)
│   ├── Game/        (Platformer.Game.asmdef)
│   └── UI/          (Platformer.UI.asmdef)
├── Arts/
│   ├── Sprites/
│   ├── Animations/
│   ├── Tilesets/
│   ├── VFX/
│   └── Audio/
├── Prefabs/
├── Resources/
├── Datas/
└── Scenes/
```

## 사용 흐름 (디자이너)

1. 스프라이트 시트(PNG) 만들어서 Arts/ 폴더에 넣기
2. AI에게 "시트 올렸어, 적용해줘" 요청
3. AI가 슬라이싱 → 애니메이션 → 씬 적용까지 처리
4. 플레이 버튼 눌러서 확인
5. "점프 높이 올려줘", "적 추가해줘" 등 자연어로 수정 요청

## 세팅 필요 사항

### Codex

`.agents/skills/`가 레포에 포함되어 있어 Codex가 프로젝트 전용 가드레일과 작업 절차를 스킬로 사용할 수 있다.

기존 `.cursor/rules/`는 Cursor 호환 원본 규칙으로 남겨둘 수 있지만, Codex 기준 가드레일은 `.agents/skills/platformer-*` 스킬이다.

### Cursor / Claude Code

`.cursor/rules/`, `.cursor/skills/`는 호환용으로 유지할 수 있다. 현재 프로젝트의 기본 AI 작업 기준은 Codex와 `.agents/skills/`다.
