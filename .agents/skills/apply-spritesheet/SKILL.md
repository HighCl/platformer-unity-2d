---
name: apply-spritesheet
description: 캐릭터/오브젝트 스프라이트 시트(PNG)를 임포트하고 슬라이싱 → 애니메이션 클립 → Animator → 씬 적용까지 처리한다. "시트 올렸어, 적용해줘" 요청 시 사용.
---

# 스프라이트 시트 적용

디자이너가 Arts/ 폴더에 PNG 시트를 넣으면, 슬라이싱부터 씬 적용까지 처리한다.

## 전제 조건

- PNG 파일이 Assets/_Project/Arts/Sprites/ 하위에 이미 있어야 함
- 에디트 모드여야 함 (플레이 모드 금지)

## Step 0 — Unity 수동 미사용

Unity 수동는 제거되었으므로 수동 모드 절차를 따른다.
코드와 파일 작성은 Codex가 수행하고, Unity 에디터 조작은 사용자에게 단계별로 안내한다.

---

## Step 1 — 임포트 설정

### 수동 모드

> 1. Project 패널에서 시트 PNG 파일 선택.
> 2. Inspector에서:
>    - Texture Type: Sprite (2D and UI)
>    - Sprite Mode: Multiple
>    - Pixels Per Unit: 16
>    - Filter Mode: Point (no filter)
>    - Compression: None
> 3. 하단 Apply 클릭.

---

## Step 2 — 슬라이싱 확인

### 수동 모드

> 1. Project 패널에서 시트 PNG 선택 → Inspector 하단 Sprite Editor 클릭.
> 2. 왼쪽 상단 Slice 드롭다운 클릭.
> 3. Type: Grid By Cell Size 선택, 셀 크기 입력 (캐릭터 한 프레임 크기).
> 4. Slice 클릭 → 우측 상단 Apply.
> 5. 닫기.

---

## Step 3 — 스프라이트 이름 파악

### 수동 모드

> Sprite Editor에서 각 슬라이스의 이름을 확인해줘.
> 예: "Idle 0", "Idle 1", "Run 0", "Run 1", "Run 2" 이런 식이면
> → Idle (2프레임), Run (3프레임)으로 그룹이 나뉘는 거야.

---

## Step 4 — 애니메이션 클립 생성

### 수동 모드

> 1. Project 패널에서 Assets/_Project/Arts/Animations/Player/ 폴더로 이동 (없으면 생성).
> 2. 폴더 우클릭 → Create → Animation Clip. 이름을 Player_Idle 으로.
> 3. Hierarchy에서 Player 오브젝트 선택 → Window → Animation → Animation 창 열기.
> 4. Animation 창 좌측 상단에서 방금 만든 클립 선택.
> 5. 타임라인에 스프라이트를 프레임 순서대로 드래그해서 배치.
> 6. Samples(FPS)를 Idle은 6, Run은 10 정도로 설정.
> 7. 각 상태(Run, Jump 등)에 대해 2~6번 반복.

---

## Step 5 — Animator Controller 생성/업데이트

### 수동 모드

> 1. Assets/_Project/Arts/Animations/Player/ 폴더 우클릭 → Create → Animator Controller. 이름: PlayerAnimator.
> 2. 더블클릭해서 Animator 창 열기.
> 3. Parameters 탭에서 + → Float → "Speed" 추가, + → Bool → "IsGrounded" 추가.
> 4. 만든 .anim 클립들을 Animator 창으로 드래그 (상태 자동 생성).
> 5. 상태 간 전환 설정:
>    - Idle → Run: Speed > 0.01
>    - Run → Idle: Speed < 0.01
>    - Any State → Jump: IsGrounded = false
>    - Jump → Idle: IsGrounded = true

---

## Step 6 — 씬에 적용

### 수동 모드

> 1. Hierarchy에서 Player 선택.
> 2. SpriteRenderer의 Sprite 슬롯에 Idle 첫 프레임 스프라이트 드래그.
> 3. 색상이 보라색이면: Material 슬롯을 Sprites/Default 로 변경.
> 4. Animator 컴포넌트가 없으면 Add Component → Animator.
> 5. Controller 슬롯에 PlayerAnimator.controller 드래그.
> 6. Ctrl+S 로 씬 저장.

---

## Step 7 — 검증

Unity 에디터에서 `Assets > Refresh` 또는 `Ctrl+R`로 변경사항을 반영한다.
`Console` 창에서 컴파일 오류를 확인하고, 테스트는 `Window > General > Test Runner`에서 실행한다.

체크리스트:
- [ ] 컴파일 에러 0
- [ ] 씬에서 스프라이트가 보라색 아닌 정상 색상
- [ ] 플레이 모드에서 Idle 애니메이션 재생
- [ ] 이동 시 Run 애니메이션 전환
- [ ] 점프 시 Jump 애니메이션 전환
- [ ] 좌우 반전 정상 작동

## 주의사항

- PPU가 기존과 다르면 캐릭터 크기가 달라진다. 반드시 16으로 맞출 것
- 시트 구조(프레임 수, 이름)가 바뀌면 애니메이션 클립을 새로 만들어야 한다
- 기존 시트를 같은 파일명으로 덮어쓰면 참조가 유지된다 (Unity GUID 기반)
