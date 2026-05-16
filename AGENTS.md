# Platformer Unity 2D

Unity 6.0.3, URP 2D 기반 2D 플랫포머 에셋 데모 프로젝트.
아트 디자이너가 AI(Codex)와 함께 작업하는 것을 전제로 구성된 가드레일 환경.

## 작업 시작 전 필수 확인

코드 변경 후에는 Unity 에디터에서 `Assets > Refresh` 또는 `Ctrl+R`로 반영하고, `Console`과 `Test Runner`로 검증한다.
씬/프리팹/에셋 조작이 필요하면 Codex가 파일을 직접 편집하지 않고, Unity 에디터에서 수행할 단계를 사용자에게 안내한다.

## Codex 스킬 규칙

Cursor의 `.cursor/rules`는 Codex에서 자동 적용되지 않는다.
Codex 작업 시에는 `.agents/skills/`의 프로젝트 전용 스킬을 사용한다.

- `platformer-guardrails`: 프로젝트 작업 시작 시 최상위 가드레일
- `platformer-architecture`: 어셈블리 경계, Game/UI 분리, Unity 직렬화 보호
- `platformer-coding-style`: C# 네이밍, 폴더 구조, Input System, 테스트 규칙
- `platformer-game-logic`: Core/Game 물리, 이동, 적 AI, 스톰프, 사망 처리 규칙
- `platformer-ui`: UI, Canvas, TMP, SO 이벤트 구독 규칙
- `platformer-unity-nondev-basics`: 비개발자용 Unity 설명과 수동 조작 안내

## 어셈블리 맵

```
Platformer.Data
    └── (참조 없음)

Platformer.Core
    └── → Platformer.Data

Platformer.Game
    └── → Platformer.Core
    └── → Platformer.Data

Platformer.UI
    └── → Platformer.Core
    └── → Platformer.Data
```

참조 테이블:

| 어셈블리        | 참조 가능                      | 참조 불가              |
|----------------|-------------------------------|----------------------|
| Platformer.Data | (없음)                        | Core, Game, UI       |
| Platformer.Core | Data                          | Game, UI             |
| Platformer.Game | Data, Core                    | UI                   |
| Platformer.UI   | Data, Core                    | Game                 |

Game ↔ UI 직접 참조 절대 금지. 서로 참조하면 빌드가 순환 참조로 터짐.

## Game ↔ UI 소통 규칙

Game과 UI는 서로 직접 참조할 수 없다. 소통은 반드시 SO 이벤트 채널을 통해.

```csharp
// Assets/_Project/Datas/Events/ 에 SO 인스턴스 생성
// GameEvent.cs (Platformer.Data 어셈블리)

// 발행 (Game쪽):
[SerializeField] private GameEvent onPlayerDied;
onPlayerDied.Raise();

// 구독 (UI쪽):
[SerializeField] private GameEvent onPlayerDied;
void OnEnable() => onPlayerDied.AddListener(OnPlayerDied);
void OnDisable() => onPlayerDied.RemoveListener(OnPlayerDied);
```

## 코드 규칙

### 1파일 1클래스
파일 하나에 클래스 하나. private inner class는 예외.

### 네임스페이스
파일이 속한 어셈블리 이름과 동일하게.
- Scripts/Data/ → namespace Platformer.Data
- Scripts/Core/ → namespace Platformer.Core
- Scripts/Game/ → namespace Platformer.Game
- Scripts/UI/ → namespace Platformer.UI

### private 메서드 네이밍
private 메서드는 `_PascalCase`로 작성.
```csharp
public void TakeDamage(int amount) { }   // public
private void _HandleDeath() { }          // private
```

### Boolean 네이밍
Boolean 변수에는 is/has 접두사.
```csharp
bool isGrounded;
bool hasCheckpoint;
```

### 코드 배치 순서 (region)
클래스 내부 배치: 상수 → 변수 → 프로퍼티 → 라이프사이클 → Public 메서드 → Private 메서드 → 엔진 콜백.
30줄 이상인 클래스에서 `#region`으로 구분.

### 클래스 접미사 규약
Controller(오브젝트 제어), Manager(전역 싱글톤), Settings(SO 설정), Data(SO 런타임), Event(SO 이벤트), Zone(트리거 영역).

### UI 오브젝트 프리픽스
씬/프리팹에서 UI 오브젝트 이름에 접두사: Txt_(텍스트), BTN_(버튼), Img_(이미지), Pan_(패널), Go_(일반), Tg_(토글).

### 프로젝트 디렉토리 구조
```
Assets/_Project/
├── Arts/        — 스프라이트, 애니메이션, 타일셋, VFX, 오디오
├── Datas/       — SO 인스턴스 (.asset)
│   └── Events/  — GameEvent SO 인스턴스
├── Prefabs/     — 프리팹
├── Resources/   — Resources.Load용 (최소한으로)
├── Scenes/      — 씬 파일
└── Scripts/     — 코드 (어셈블리별 분리)
```
코드는 Scripts/, 에셋은 Arts/, SO는 Datas/, 외부 에셋팩은 Assets/ 바로 아래.

### Unity 직렬화 파일 직접 수정 금지

.unity(씬), .prefab, .asset, .controller, .anim 파일을 텍스트로 열어서 직접 읽거나 수정하지 말 것.
이 파일들은 Unity YAML Serialization 포맷이라 fileID/GUID 참조가 얽혀 있어서, 직접 편집하면 참조가 깨진다.

씬/프리팹/에셋 조작이 필요하면:
- 사용자에게 Unity 에디터 조작을 단계별로 안내
- 자동화가 꼭 필요하면 Unity Editor 스크립트를 별도 코드로 작성하되, `.unity`, `.prefab`, `.asset`, `.controller`, `.anim` 파일을 직접 텍스트 편집하지 않는다

유일한 예외: SO(.asset)의 단순 값 수정은 YAML 직접 수정 후 Unity 에디터 Refresh 허용 (ScriptableObject 수정 주의사항 참고).

### Arts/ 폴더 스크립트 금지
Arts/ 하위에는 스프라이트, 애니메이션, 타일셋, VFX, 오디오 에셋만. .cs 파일 절대 넣지 말 것.

### SO Settings 패턴 (하드코딩 금지)
숫자 값(속도, 점프력, 체력 등)은 반드시 ScriptableObject로 분리.

```csharp
// 나쁜 예
float speed = 5f;

// 좋은 예
[SerializeField] private PlayerSettings settings;
float speed = settings.moveSpeed;
```

SO 인스턴스는 Assets/_Project/Datas/ 에 저장.

### Input System
레거시 Input (Input.GetKey 등) 사용 금지. 반드시 Unity Input System 패키지 사용.

```csharp
// 금지
if (Input.GetKeyDown(KeyCode.Space)) { ... }

// 올바른 방법
// PlayerInputActions.inputactions 에셋 → Generate C# Class 체크
private PlayerInputActions _input;
void Awake() => _input = new PlayerInputActions();
void OnEnable() => _input.Enable();
void OnDisable() => _input.Disable();
```

## Unity 에디터 검증

코드 수정 후 반드시 Unity 에디터에 변경사항을 반영하고 컴파일 상태를 확인할 것.

```powershell
# 컴파일 에러 확인
Get-Content "$env:LOCALAPPDATA\Unity\Editor\Editor.log" -Tail 300 | Select-String "error CS"

# 런타임 에러 확인
Get-Content "$env:LOCALAPPDATA\Unity\Editor\Editor.log" -Tail 300 | Select-String "NullReferenceException|InvalidOperationException|MissingReferenceException"

# 경고 확인
Get-Content "$env:LOCALAPPDATA\Unity\Editor\Editor.log" -Tail 300 | Select-String "warning CS"
```

검증 흐름:
1. 코드 수정
2. Unity 에디터에서 `Assets > Refresh` 또는 `Ctrl+R` 실행
3. `Console` 창 또는 `Editor.log`에서 컴파일 에러 확인
4. 에러 있으면 수정 후 1번부터 반복
5. `Window > General > Test Runner`에서 테스트 통과 확인

## 테스트

테스트 실행:
- Unity 메뉴 `Window > General > Test Runner` 열기
- `Run All` 실행

테스트 파일 위치: 각 어셈블리의 Tests/ 폴더
- Scripts/Data/Tests/
- Scripts/Core/Tests/
- Scripts/Game/Tests/
- Scripts/UI/Tests/

새 기능 추가 시 대응하는 테스트도 함께 작성할 것. 테스트 없이 머지하지 말 것.

## ScriptableObject 수정 주의사항

assets-modify로 SO를 부분 수정하면 명시하지 않은 필드가 기본값(0)으로 리셋될 수 있다.

SO 에셋 수정이 필요할 때:
- 안전한 방법: YAML 직접 수정 + Unity 에디터 Refresh 조합
- assets-modify는 전체 필드를 다 지정할 때만 사용

## 커밋 컨벤션

```
<type>: <설명>
```

타입:
- feat — 새 기능
- fix — 버그 수정
- art — 에셋 추가/수정 (스프라이트, 애니메이션 등)
- docs — 문서
- chore — 기타

예시:
- feat: 이중 점프 구현
- art: 플레이어 달리기 스프라이트 교체
- fix: 경사면 미끄러짐 수정
