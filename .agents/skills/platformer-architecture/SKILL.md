---
name: platformer-architecture
description: Platformer Unity 2D 프로젝트의 어셈블리 경계, namespace와 폴더 대응, Game/UI 직접 참조 금지, SO 이벤트 채널, Unity 직렬화 파일 보호, 수동 씬/프리팹/에셋 조작, 플레이 모드 안전 규칙을 적용한다. 코드 작성, 리팩터링, 리뷰, 씬/프리팹/에셋 작업, 어셈블리 참조 변경, Game과 UI 사이 통신 구현 시 사용한다.
---

# 어셈블리 경계 규칙

## Unity 에디터 조작 원칙

- 코드는 Codex가 작성하되, 씬/프리팹/Inspector 조작은 사용자에게 단계별로 안내한다.
- Unity 직렬화 파일은 텍스트로 열거나 직접 수정하지 않는다.
- 코드 수정 후 Unity 에디터에서 `Assets > Refresh` 또는 `Ctrl+R`로 반영한다.

## 참조 방향

```text
Platformer.Data  <-  Platformer.Core  <-  Platformer.Game
                 <-  Platformer.UI
```

- `Platformer.Data`: 아무것도 참조하지 않는다.
- `Platformer.Core`: `Platformer.Data`만 참조한다.
- `Platformer.Game`: `Platformer.Data`, `Platformer.Core`만 참조한다.
- `Platformer.UI`: `Platformer.Data`, `Platformer.Core`만 참조한다.
- `Platformer.Game`과 `Platformer.UI`는 서로 참조하지 않는다.

## namespace와 폴더 대응

| namespace | 폴더 |
|---|---|
| `Platformer.Data` | `Assets/_Project/Scripts/Data/` |
| `Platformer.Core` | `Assets/_Project/Scripts/Core/` |
| `Platformer.Game` | `Assets/_Project/Scripts/Game/` |
| `Platformer.UI` | `Assets/_Project/Scripts/UI/` |

파일을 만들 때 폴더와 namespace를 반드시 일치시킨다.

## Game ↔ UI 이벤트 채널

Game과 UI는 직접 참조하지 않는다. SO 이벤트(`GameEvent`)로만 소통한다.

```csharp
// 이벤트 발행 (Game 쪽)
[SerializeField] private GameEvent _onPlayerDied;
_onPlayerDied.Raise();

// 이벤트 구독 (UI 쪽)
[SerializeField] private GameEvent _onPlayerDied;
void OnEnable() => _onPlayerDied.AddListener(_HandlePlayerDied);
void OnDisable() => _onPlayerDied.RemoveListener(_HandlePlayerDied);
private void _HandlePlayerDied() { }
```

- SO 인스턴스는 `Assets/_Project/Datas/Events/`에 저장한다.
- 인스턴스는 Inspector에서 연결해야 작동한다.

## Unity 직렬화 파일 직접 접근 금지

다음 파일은 텍스트 에디터로 열어서 읽거나 수정하지 않는다.

- `.unity`
- `.prefab`
- `.asset`
- `.controller`
- `.anim`

이 파일들은 Unity YAML Serialization 포맷이며 `fileID`/`GUID` 참조가 얽혀 있다. 직접 편집하면 참조가 깨질 수 있다.

씬/하이라키/타일맵 정보가 필요하면 사용자에게 Unity 에디터에서 확인할 항목을 단계별로 요청한다.

씬/프리팹/에셋 수정이 필요하면:

- 사용자에게 Unity 에디터 조작 절차를 안내한다.
- 예외: SO `.asset`의 단순 값 수정은 YAML 수정 후 Unity 에디터 Refresh를 허용한다.

## 금지 패턴

```csharp
// 씬 전체 검색 금지
GameObject.Find("PlayerObject");
FindObjectOfType<PlayerController>();
FindObjectsOfType<Enemy>();

// UI 코드에서 Game 네임스페이스 참조 금지
using Platformer.Game;

// Game 코드에서 UI 네임스페이스 참조 금지
using Platformer.UI;
```

대안은 Inspector `[SerializeField]` 직접 연결 또는 SO 이벤트 채널이다.

## 플레이 모드 규칙

- 씬 오브젝트 수정, 씬 저장, 에셋 생성은 에디트 모드에서 수행한다.
- 플레이 모드에서 `MarkSceneDirty`, `CreateAsset` 등을 호출하지 않는다.
- 플레이 모드 중 변경한 값은 플레이 종료 시 사라진다.

씬 수정 작업 전 확인 예시:

```csharp
if (UnityEditor.EditorApplication.isPlaying)
{
    UnityEditor.EditorApplication.isPlaying = false;
    return "플레이 모드 종료 후 다시 실행";
}
```

## 검증 흐름

1. 코드 수정
2. Unity 에디터에서 `Assets > Refresh` 또는 `Ctrl+R` 실행
3. `Console` 창 또는 `Editor.log`에서 컴파일 에러 확인
4. 에러가 있으면 수정 후 반복
5. 필요 시 런타임 에러도 확인

```powershell
Get-Content "$env:LOCALAPPDATA\Unity\Editor\Editor.log" -Tail 300 | Select-String "NullReferenceException|InvalidOperationException|MissingReferenceException"
```

에러 0을 확인할 때까지 다음 작업으로 넘어가지 않는다.

## 커밋 컨벤션

```text
<type>: <설명>
```

- `feat`: 새 기능
- `fix`: 버그 수정
- `art`: 에셋 추가/수정
- `docs`: 문서
- `chore`: 기타
