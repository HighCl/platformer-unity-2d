---
name: platformer-unity-nondev-basics
description: Platformer Unity 2D 프로젝트에서 비개발자에게 Unity 용어, ScriptableObject 조작법, 자주 나는 Unity/컴파일/런타임 에러 의미, 수동 조작법, 스프라이트 교체 절차를 쉽게 설명할 때 사용한다. 사용자가 오류 메시지 해석, Unity 에디터에서 무엇을 눌러야 하는지, Inspector/Prefab/Scene/Tilemap 같은 용어 설명을 요청할 때 사용한다.
---

# Unity 기초 설명

## 자주 나오는 Unity 용어

| 용어 | 쉬운 설명 |
|---|---|
| Scene | 게임의 한 화면/레벨. 오브젝트를 배치하는 곳 |
| GameObject | 씬에 존재하는 모든 것. 플레이어, 적, 카메라 포함 |
| Component | GameObject에 붙이는 기능 모듈. Rigidbody, Collider 등 |
| Prefab | 재사용 가능한 GameObject 템플릿 |
| ScriptableObject (SO) | 씬에 없지만 데이터를 저장하는 에셋. 설정값이나 이벤트 채널로 사용 |
| Inspector | 선택한 오브젝트의 속성을 보고 수정하는 패널 |
| SerializeField | 코드의 private 변수를 Inspector에 보이게 하는 속성 |
| Animator | 애니메이션 상태를 관리하는 컴포넌트 |
| Tilemap | 타일 기반 배경을 그리는 시스템 |
| URP | Universal Render Pipeline. 이 프로젝트의 렌더링 방식 |

## 자주 나는 에러

| 에러 메시지 | 의미 | 해결 |
|---|---|---|
| `NullReferenceException` | 연결해야 할 참조가 비어 있음 | Inspector에서 빈 슬롯 확인 |
| `MissingReferenceException` | 연결된 오브젝트가 삭제됨 | 참조 오브젝트가 씬에 있는지 확인 |
| `Assembly ... does not exist` | `.asmdef` 이름이 틀림 | `.asmdef`의 `name` 필드 확인 |
| `CS0246: type not found` | using 문이 빠졌거나 어셈블리 참조 없음 | namespace와 asmdef 참조 확인 |
| `The type ... exists in both` | 같은 클래스 이름이 중복됨 | 파일/클래스 이름 중복 확인 |

## ScriptableObject 설정 조작

1. Project 패널에서 해당 `.asset` 파일을 클릭한다.
2. Inspector에서 값을 수정한다.
3. `Ctrl+S`로 저장한다.
4. 플레이 모드 중 수정한 값은 종료 시 사라질 수 있으므로 에디트 모드에서 수정한다.

## AI가 코드를 수정할 때

Codex가 "리프레시 확인"이라고 하면 Unity 에디터에 변경사항을 반영하고 컴파일 에러를 확인하는 단계다.

컴파일 에러가 있으면:

- Unity 에디터 하단에 빨간 에러 아이콘이 보인다.
- 플레이 버튼이 눌리지 않을 수 있다.
- Codex가 에러를 수정할 때까지 기다리면 된다.

## AI가 씬/프리팹을 조작해야 할 때

- Codex는 `.unity`, `.prefab` 파일을 직접 텍스트로 편집하지 않는다.
- Codex가 Unity 에디터에서 수행할 단계를 안내한다.

## 자주 생기는 문제

| 증상 | 원인 | 해결 |
|---|---|---|
| 캐릭터가 보라색으로 보임 | URP Lit 셰이더 문제 | SpriteRenderer 머티리얼을 `Sprites/Default`로 변경 |
| 점프가 안 됨 | Ground 레이어 미설정 | 타일맵 Layer와 PlayerController GroundLayer 확인 |
| 이동 중 갑자기 멈춤 | 타일 이음새 콜라이더 걸림 | 타일맵에 `CompositeCollider2D` 추가 |
| 벽에 달라붙음 | 콜라이더 마찰력 | 플레이어에 NoFriction 머티리얼 적용 |
| W+D 입력 시 느려짐 | 대각선 입력 정규화 | 코드에서 x축 방향만 사용 |
| 씬 수정 중 에러 | 플레이 모드 중 수정 시도 | 플레이를 멈추고 에디트 모드에서 수정 |
| 플레이 중 바꾼 값이 사라짐 | 플레이 모드 변경은 임시 | 에디트 모드에서 변경 |
| 캐릭터가 뒤로 걸음 | 좌우 반전 방식 문제 | `transform.localScale` 대신 `SpriteRenderer.flipX` 사용 |
| 적이 좌우로 흔들림 | 추적 도착 지점 데드존 없음 | `stopDistance` 추가 |
| 적 밟기가 안 먹힘 | `bounds` 비교가 부정확 | 충돌 노멀 기반 판정 사용 |
| 죽었는데 애니메이션이 안 나옴 | `simulated=false`가 Animator도 멈춤 | constraints와 `gravityScale=0`으로 동결 |

## 스프라이트 교체 빠른 순서

1. 새 이미지 파일을 `Assets/_Project/Arts/Sprites/`에 복사한다.
2. Unity가 자동 임포트했는지 Project 패널에서 확인한다.
3. 임포트된 스프라이트 선택 후 Inspector에서 `Texture Type: Sprite (2D and UI)`를 확인한다.
4. Sprite Renderer 또는 Animator에서 기존 스프라이트를 새 것으로 교체한다.
