---
name: platformer-ui
description: Platformer Unity 2D 프로젝트의 UI 코드와 Canvas 작업 규칙. Assets/_Project/Scripts/UI 코드, HUD, 메뉴, 팝업, Canvas 계층, TextMeshPro, UI 이벤트 구독, Game 네임스페이스 직접 참조 금지, SO 이벤트 채널 기반 UI 갱신을 구현하거나 리뷰할 때 사용한다.
---

# UI 규칙

## Game 네임스페이스 참조 금지

`Assets/_Project/Scripts/UI/` 하위 코드에서는 `Platformer.Game`을 참조하지 않는다.

```csharp
// 금지
using Platformer.Game;
```

UI가 게임 상태를 알아야 할 때는 SO 이벤트를 구독한다.

## SO 이벤트 구독 패턴

```csharp
using UnityEngine;
using Platformer.Data;

namespace Platformer.UI
{
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private GameEvent _onPlayerDied;
        [SerializeField] private GameEvent _onHealthChanged;

        void OnEnable()
        {
            _onPlayerDied.AddListener(_HandlePlayerDied);
            _onHealthChanged.AddListener(_HandleHealthChanged);
        }

        void OnDisable()
        {
            _onPlayerDied.RemoveListener(_HandlePlayerDied);
            _onHealthChanged.RemoveListener(_HandleHealthChanged);
        }

        private void _HandlePlayerDied() { }
        private void _HandleHealthChanged() { }
    }
}
```

- 구독은 `OnEnable`에서 한다.
- 해제는 `OnDisable`에서 한다.
- 구독/해제 쌍이 없으면 씬 전환 시 오류가 생길 수 있다.

## TextMeshPro

텍스트는 TMP를 사용한다. 레거시 `Text` 컴포넌트를 추가하지 않는다.

```csharp
// 금지
using UnityEngine.UI;
Text _label;

// 올바른 방식
using TMPro;
[SerializeField] private TMP_Text _label;
[SerializeField] private TMP_Text _scoreText;
```

## Canvas 계층

Canvas 하나에 모든 오브젝트를 직접 넣지 않는다.

```text
Canvas (Screen Space - Overlay)
├── HUD/
├── Menus/
└── Popups/
```

- `HUD`: 체력바, 점수 등 항상 표시되는 요소
- `Menus`: 일시정지, 설정 등 토글되는 화면
- `Popups`: 다이얼로그, 알림 등 최상위 화면

표시/숨김은 그룹 GameObject 단위로 `SetActive`를 사용한다.
