---
name: add-feature
description: 만들어줘, 추가해줘, 구현해줘 요청 시 사용. 적, 플랫폼, 함정, 기믹, 아이템, UI, 체력바, 발사체 등 새 게임 기능을 추가할 때 이 절차를 따른다. 코드 작성, 수동 프리팹/씬 적용 안내, 테스트까지 포함한다.
---

# 새 기능 추가

기능 요청을 받으면 아래 절차를 순서대로 실행한다.

## 전제 조건

- Unity 프로젝트가 열려있고 에디트 모드여야 한다.
- 어셈블리 맵을 숙지한다. (`AGENTS.md`, `platformer-architecture` 참고)
- 코드는 Codex가 작성하고, 프리팹/씬/Inspector 조작은 사용자에게 Unity 에디터 절차로 안내한다.

## Step 0 — 스펙 확인

코드를 작성하기 전에 아래 항목을 사용자에게 확인한다.

1. 기능의 동작 방식
2. 기존 오브젝트와의 상호작용
3. 겉모습과 사용할 스프라이트
4. 속도, 체력, 데미지 같은 주요 설정값

사용자가 "알아서 해"라고 하면 기본값을 제안하고 승인받은 뒤 진행한다.

## Step 1 — 어셈블리 판단

| 질문 | 어셈블리 |
|---|---|
| 데이터 구조, 설정값, 이벤트 정의 | Platformer.Data |
| 순수 게임 규칙, 인터페이스 | Platformer.Core |
| 플레이어/적/월드 동작, 물리 | Platformer.Game |
| 화면에 보이는 UI 요소 | Platformer.UI |

- Game과 UI는 직접 참조하지 않는다.
- Game과 UI 소통이 필요하면 Data에 SO 이벤트 채널을 추가한다.

## Step 2 — SO Settings 정의

숫자 값은 하드코딩하지 않고 ScriptableObject로 분리한다.

```csharp
using UnityEngine;

namespace Platformer.Data
{
    [CreateAssetMenu(fileName = "XxxSettings", menuName = "Platformer/Settings/Xxx")]
    public class XxxSettings : ScriptableObject
    {
        public float moveSpeed = 3f;
        public int maxHealth = 3;
    }
}
```

SO 인스턴스가 필요하면 사용자에게 안내한다.

> Project 패널에서 `Assets/_Project/Datas/` 폴더 우클릭 → `Create > Platformer > Settings > Xxx` 선택.

## Step 3 — 이벤트 채널 추가

Game과 UI 사이 통신이 필요하면 `GameEvent` SO 인스턴스를 사용한다.

> `Assets/_Project/Datas/Events/` 폴더 우클릭 → `Create > Platformer > Events > Game Event` 선택 후 이름 지정.

## Step 4 — 스크립트 작성

- 파일 위치와 namespace를 어셈블리와 일치시킨다.
- 1파일 1클래스를 지킨다.
- SO Settings는 `[SerializeField] private`으로 주입한다.
- 레거시 `Input`은 사용하지 않고 Unity Input System을 사용한다.

## Step 5 — 컴파일 확인

사용자에게 Unity 에디터 검증을 안내한다.

> 1. Unity 에디터에서 `Assets > Refresh` 또는 `Ctrl+R` 실행.
> 2. `Console` 창에서 빨간 컴파일 에러가 0개인지 확인.
> 3. 에러가 있으면 메시지를 전달.

PowerShell 로그 확인이 필요하면 다음 명령을 사용한다.

```powershell
Get-Content "$env:LOCALAPPDATA\Unity\Editor\Editor.log" -Tail 300 | Select-String "error CS"
```

## Step 6 — 프리팹 생성/수정 안내

MonoBehaviour를 만들었으면 사용자에게 프리팹 조작을 안내한다.

> 1. Hierarchy 창에서 우클릭 → `Create Empty`.
> 2. 오브젝트 이름을 기능명에 맞게 변경.
> 3. Inspector 하단 `Add Component`에서 새 Controller 추가.
> 4. 필요한 경우 `Rigidbody2D`, `BoxCollider2D`, `SpriteRenderer` 추가.
> 5. 화면에 보이는 오브젝트라면 SpriteRenderer에 스프라이트 연결.
> 6. Hierarchy의 오브젝트를 `Assets/_Project/Prefabs/` 폴더로 드래그해 프리팹 생성.

## Step 7 — Inspector 와이어링 안내

> 1. Project 패널에서 프리팹을 더블클릭해 프리팹 편집 모드 진입.
> 2. 새 컴포넌트의 Settings/Event 슬롯 확인.
> 3. `Assets/_Project/Datas/` 또는 `Assets/_Project/Datas/Events/`의 에셋을 슬롯에 드래그.
> 4. 상단 뒤로 가기 버튼으로 나가며 저장.

## Step 8 — 테스트 작성

새 기능에 대응하는 테스트를 작성한다.

- `Scripts/Data/Tests/`
- `Scripts/Core/Tests/`
- `Scripts/Game/Tests/`
- `Scripts/UI/Tests/`

테스트 실행은 사용자에게 안내한다.

> Unity 메뉴 `Window > General > Test Runner` 열기 → `Run All` 클릭.

## Step 9 — 씬 배치 안내

프리팹을 씬에 배치해야 하면 사용자에게 안내한다.

> 1. Project 패널에서 프리팹을 Scene 뷰 또는 Hierarchy로 드래그.
> 2. Inspector의 Transform Position 값을 조정.
> 3. `Ctrl+S`로 씬 저장.

## Step 10 — 최종 검증

- 컴파일 에러 0개 확인
- 테스트 통과 확인
- 어셈블리 경계 위반 없음
- SerializeField 슬롯 연결 확인
- 플레이 모드에서 동작 확인
