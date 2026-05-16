---
name: platformer-coding-style
description: Platformer Unity 2D 프로젝트의 C# 코딩 스타일, 파일/클래스 구조, 네이밍, region 배치, SerializeField 정책, SO Settings 패턴, Input System 사용, UI 오브젝트 프리픽스, 프로젝트 디렉토리 구조, 컴파일 확인, 테스트 작성, Debug.Log 정책을 적용한다. C# 코드 작성/수정/리뷰, 새 스크립트 생성, 폴더 배치, 테스트 추가 시 사용한다.
---

# 코딩 스타일

## 1파일 1클래스

- 파일 하나에는 클래스 하나만 둔다.
- `private` 내부 클래스는 예외로 허용한다.

## Arts 폴더 스크립트 금지

- `Assets/_Project/Arts/` 하위에는 스프라이트, 애니메이션, 타일셋, VFX, 오디오 에셋만 둔다.
- `.cs` 파일은 절대 넣지 않는다.

## SO Settings 패턴

숫자 값은 하드코딩하지 않고 ScriptableObject로 분리한다.

```csharp
// 금지
float speed = 5f;

// 올바른 방식
[SerializeField] private PlayerSettings _settings;
float speed = _settings.moveSpeed;
```

- SO 클래스는 `Assets/_Project/Scripts/Data/`에 둔다.
- SO 인스턴스는 `Assets/_Project/Datas/`에 저장한다.

## 네이밍

| 대상 | 규칙 | 예시 |
|---|---|---|
| 클래스, 프로퍼티 | PascalCase | `PlayerController`, `MoveSpeed` |
| public/protected 메서드 | PascalCase | `TakeDamage()`, `GetHealth()` |
| private 메서드 | `_PascalCase` | `_HandleDeath()`, `_RebuildEndpoints()` |
| 파라미터, 로컬 변수 | camelCase | `jumpForce`, `isGrounded` |
| private 필드 | `_camelCase` | `_rigidbody`, `_isJumping` |
| SerializeField | `_camelCase` | `_moveSpeed`, `_jumpHeight` |
| 상수 | UPPER_SNAKE | `MAX_JUMP_COUNT` |
| 인터페이스 | I + PascalCase | `IDamageable`, `IInteractable` |
| Boolean | is/has 접두사 | `isGrounded`, `hasCheckpoint` |
| 컬렉션 | 복수형 또는 접미사 | `enemies`, `priceList`, `itemDic` |

## 클래스 접미사

| 접미사 | 용도 | 예시 |
|---|---|---|
| `Controller` | 오브젝트 단위 동작 제어 | `PlayerController` |
| `Manager` | 전역 싱글톤 | `GameManager` |
| `Settings` | SO 설정 데이터 | `MovingPlatformSettings` |
| `Data` | SO 런타임 데이터 | `RespawnData` |
| `Event` | SO 이벤트 채널 | `GameEvent` |
| `Zone` | 트리거 영역 | `EnemyStompZone` |

## 코드 배치 순서

30줄 이상 클래스는 region으로 구분한다.

```csharp
public class ExampleController : MonoBehaviour
{
    #region 상수
    private const float MAX_SPEED = 10f;
    #endregion

    #region 변수
    [SerializeField] private ExampleSettings _settings;
    private float _currentSpeed;
    private bool _isActive;
    #endregion

    #region 프로퍼티
    public float CurrentSpeed => _currentSpeed;
    #endregion

    #region 유니티 라이프사이클
    void Awake() { }
    void OnEnable() { }
    void Start() { }
    void Update() { }
    void FixedUpdate() { }
    void OnDisable() { }
    void OnDestroy() { }
    #endregion

    #region Public 메서드
    public void Activate() { }
    #endregion

    #region Private 메서드
    private void _HandleMovement() { }
    #endregion

    #region 엔진 콜백
    void OnCollisionEnter2D(Collision2D col) { }
    void OnTriggerEnter2D(Collider2D col) { }
    #endregion
}
```

짧은 클래스에는 불필요한 region을 만들지 않는다.

## SerializeField

- public 필드는 쓰지 않는다.
- 에디터 노출이 필요하면 `[SerializeField] private`을 사용한다.

```csharp
// 금지
public float moveSpeed = 5f;

// 올바른 방식
[SerializeField] private float _moveSpeed = 5f;
```

## Input System

레거시 `Input` 클래스는 사용하지 않는다.

```csharp
// 금지
if (Input.GetKeyDown(KeyCode.Space)) { }
float h = Input.GetAxis("Horizontal");

// 올바른 방식
private InputSystem_Actions _input;
void Awake() => _input = new InputSystem_Actions();
void OnEnable() => _input.Enable();
void OnDisable() => _input.Disable();

float h = _input.Player.Move.ReadValue<Vector2>().x;
bool jumped = _input.Player.Jump.WasPressedThisFrame();
```

## UI 오브젝트 프리픽스

| 프리픽스 | 대상 | 예시 |
|---|---|---|
| `Txt_` | TMP_Text | `Txt_Score` |
| `BTN_` | Button | `BTN_Start` |
| `Img_` | Image | `Img_HealthBar` |
| `Pan_` | Panel | `Pan_GameOver` |
| `Go_` | 일반 GameObject | `Go_SpawnPoint` |
| `Tg_` | Toggle | `Tg_Sound` |
| `Input_` | InputField | `Input_PlayerName` |
| `SV_` | ScrollView | `SV_LevelSelect` |

그룹 예시:

```text
Canvas
├── GROUP_HUD/
├── GROUP_Menus/
└── GROUP_Popups/
```

## 프로젝트 디렉토리 구조

```text
Assets/_Project/
├── Arts/
│   ├── Animations/
│   ├── Audio/
│   ├── Sprites/
│   ├── Tilesets/
│   └── VFX/
├── Datas/
│   └── Events/
├── Prefabs/
├── Resources/
├── Scenes/
└── Scripts/
    ├── Core/
    │   └── Tests/
    ├── Data/
    │   └── Tests/
    ├── Game/
    │   └── Tests/
    └── UI/
        └── Tests/
```

- 코드는 `Scripts/` 하위 어셈블리 폴더에 둔다.
- 에셋은 `Arts/` 하위에 둔다.
- SO 인스턴스는 `Datas/`에 둔다.
- 외부 에셋팩은 `Assets/_Project` 밖의 `Assets/` 바로 아래에 둔다.

## 컴파일 확인

코드 작성/수정 후 반드시 Unity에 반영하고 컴파일 상태를 확인한다.

```powershell
Get-Content "$env:LOCALAPPDATA\Unity\Editor\Editor.log" -Tail 300 | Select-String "error CS"
```

Unity 에디터에서는 `Assets > Refresh` 또는 `Ctrl+R`로 변경사항을 먼저 반영한다.
에러 0이 확인될 때까지 다음 작업으로 넘어가지 않는다.

## 테스트 규칙

새 스크립트 작성 시 대응 테스트 파일도 생성한다.

- `Scripts/Data/새기능.cs` -> `Scripts/Data/Tests/새기능Tests.cs`
- `Scripts/Core/새기능.cs` -> `Scripts/Core/Tests/새기능Tests.cs`
- `Scripts/Game/새기능.cs` -> `Scripts/Game/Tests/새기능Tests.cs`
- `Scripts/UI/새기능.cs` -> `Scripts/UI/Tests/새기능Tests.cs`

테스트 namespace는 `Platformer.X.Tests`를 사용하고 NUnit `[Test]`를 사용한다.

## Debug.Log 정책

- 임시 `Debug.Log`는 개발 중에만 사용하고 빌드 전 제거하거나 조건부 컴파일로 감싼다.
- `Debug.LogWarning`, `Debug.LogError`는 필요한 경우 허용한다.

```csharp
#if UNITY_EDITOR
Debug.Log($"[PlayerController] Jump triggered, velocity: {_rb.linearVelocity}");
#endif

Debug.LogWarning("Settings asset is null");
Debug.LogError("Critical: Rigidbody missing");
```
