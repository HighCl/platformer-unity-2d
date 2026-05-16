---
name: apply-spritesheet
description: 캐릭터/오브젝트 스프라이트 시트(PNG)를 임포트하고 슬라이싱, 애니메이션 클립, Animator, 씬 적용까지 처리한다. "시트 올렸어, 적용해줘" 요청 시 사용한다.
---

# 스프라이트 시트 적용

디자이너가 `Arts/` 폴더에 PNG 시트를 넣으면, Codex는 파일 배치와 코드/문서 작업을 처리하고 Unity 에디터 조작은 단계별로 안내한다.

## 전제 조건

- PNG 파일이 `Assets/_Project/Arts/Sprites/` 하위에 있어야 한다.
- Unity 에디터가 에디트 모드여야 한다.
- 직접 편집 금지 파일(`.anim`, `.controller`, `.prefab`, `.unity`)은 텍스트로 수정하지 않는다.

## Step 1 — 임포트 설정

사용자에게 안내한다.

> 1. Project 패널에서 시트 PNG 파일 선택.
> 2. Inspector에서 `Texture Type`을 `Sprite (2D and UI)`로 변경.
> 3. `Sprite Mode`를 `Multiple`로 변경.
> 4. `Pixels Per Unit`을 `16`으로 설정.
> 5. `Filter Mode`를 `Point (no filter)`로 설정.
> 6. `Compression`을 `None`으로 설정.
> 7. `Apply` 클릭.

## Step 2 — 슬라이싱

> 1. Project 패널에서 시트 PNG 선택.
> 2. Inspector 하단 `Sprite Editor` 클릭.
> 3. `Slice` 드롭다운 클릭.
> 4. `Type: Grid By Cell Size` 선택.
> 5. 프레임 셀 크기 입력. 예: `16 x 16`, `32 x 32`.
> 6. `Slice` 클릭 후 `Apply`.
> 7. Sprite Editor 닫기.

## Step 3 — 스프라이트 이름 확인

> Sprite Editor에서 슬라이스 이름을 확인한다.
> 예: `Idle_0`, `Idle_1`, `Run_0`, `Run_1`.

Codex는 이름 패턴을 기준으로 애니메이션 그룹을 정한다.

- `Idle`: 대기
- `Run`: 이동
- `Jump`: 점프
- `Fall`: 낙하
- `Attack`: 공격
- `Death`: 사망

## Step 4 — 애니메이션 클립 생성

사용자에게 안내한다.

> 1. Project 패널에서 `Assets/_Project/Arts/Animations/Player/` 폴더로 이동.
> 2. 폴더가 없으면 생성.
> 3. 폴더 우클릭 → `Create > Animation Clip`.
> 4. 예: `Player_Idle`, `Player_Run`, `Player_Jump`.
> 5. Hierarchy에서 대상 오브젝트 선택.
> 6. `Window > Animation > Animation` 창 열기.
> 7. 생성한 클립을 선택하고 프레임 순서대로 스프라이트를 드래그.
> 8. Samples는 Idle 6, Run 10, Jump 8 정도를 기준으로 설정.

## Step 5 — Animator Controller 설정

사용자에게 안내한다.

> 1. `Assets/_Project/Arts/Animations/Player/` 폴더 우클릭.
> 2. `Create > Animator Controller` 선택.
> 3. 이름을 `PlayerAnimator`로 지정.
> 4. 더블클릭해서 Animator 창 열기.
> 5. Parameters에 `Speed`(Float), `IsGrounded`(Bool) 추가.
> 6. 생성한 애니메이션 클립을 Animator 창에 드래그.
> 7. 전환 조건을 설정.

전환 기준:

- Idle → Run: `Speed > 0.01`
- Run → Idle: `Speed < 0.01`
- Any State → Jump: `IsGrounded = false`
- Jump → Idle: `IsGrounded = true`

## Step 6 — 씬/프리팹 적용

사용자에게 안내한다.

> 1. Hierarchy 또는 Prefab 편집 모드에서 대상 오브젝트 선택.
> 2. `SpriteRenderer`의 Sprite 슬롯에 Idle 첫 프레임 연결.
> 3. 색상이 보라색이면 Material을 `Sprites/Default`로 변경.
> 4. `Animator` 컴포넌트가 없으면 추가.
> 5. Animator의 Controller 슬롯에 `PlayerAnimator.controller` 연결.
> 6. `Ctrl+S`로 저장.

## Step 7 — 검증

- Unity 에디터에서 `Assets > Refresh` 또는 `Ctrl+R` 실행
- Console 창에서 컴파일 에러 0개 확인
- 플레이 모드에서 Idle 애니메이션 재생 확인
- 이동 시 Run 전환 확인
- 점프 시 Jump 전환 확인
- 좌우 반전 정상 작동 확인

## 주의사항

- PPU가 기존과 다르면 캐릭터 크기가 달라지므로 16으로 맞춘다.
- 시트 구조가 바뀌면 애니메이션 클립을 다시 설정해야 한다.
- 기존 파일을 같은 파일명으로 덮어쓰면 Unity GUID 참조가 유지된다.
