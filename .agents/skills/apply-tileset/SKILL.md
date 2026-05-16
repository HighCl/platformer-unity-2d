---
name: apply-tileset
description: 타일셋 스프라이트 시트(PNG)를 임포트하고 슬라이싱 → Tile 에셋 생성 → 타일맵 적용까지 처리한다. "타일셋 올렸어, 적용해줘" 요청 시 사용.
---

# 타일셋 적용

디자이너가 타일셋 시트 PNG를 넣으면, Tile 에셋 생성부터 타일맵 배치까지 처리한다.

## 전제 조건

- PNG 파일이 Assets/_Project/Arts/Tilesets/ 또는 에셋팩 폴더에 있어야 함
- 에디트 모드여야 함 (플레이 모드 금지)

## Step 0 — Unity 수동 미사용

Unity 수동는 제거되었으므로 수동 모드 절차를 따른다.
코드와 파일 작성은 Codex가 수행하고, Unity 에디터 조작은 사용자에게 단계별로 안내한다.

---

## Step 1 — 임포트 설정

### 수동 모드

> 1. Project 패널에서 타일셋 PNG 선택.
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

> 1. Project 패널에서 타일셋 PNG 선택 → Inspector 하단 Sprite Editor 클릭.
> 2. Slice → Type: Grid By Cell Size → 셀 크기 16x16 입력.
> 3. Slice 클릭 → Apply.

---

## Step 3 — 스프라이트 이름 패턴 파악

### 수동 모드

> Sprite Editor에서 각 슬라이스 이름을 확인해줘.
> 이름 패턴으로 용도를 분류할 수 있어.

---

## Step 4 — Tile 에셋 생성

### 수동 모드

> Tile 에셋은 수동으로 만들기 번거로우니, Tile Palette를 쓰는 게 낫다.
> 1. Window → 2D → Tile Palette 열기.
> 2. Create New Palette → 이름 입력 → Create.
> 3. Project 패널에서 슬라이스된 스프라이트를 Tile Palette 창으로 드래그.
> 4. 저장 위치를 Assets/_Project/Arts/Tilesets/ 로 지정.
> 5. Tile 에셋이 자동 생성된다.

---

## Step 5 — 타일맵 오브젝트 확인/생성

### 수동 모드

> 씬에 Grid 오브젝트가 없으면:
> 1. Hierarchy 우클릭 → 2D Object → Tilemap → Rectangular.
> 2. 자동으로 Grid + Tilemap 오브젝트가 생성된다.
> 3. Tilemap 오브젝트 이름을 Tilemap_Ground 으로 변경.
> 4. Inspector에서 Add Component:
>    - TilemapCollider2D (Composite Operation: Merge)
>    - Rigidbody2D (Body Type: Static)
>    - CompositeCollider2D
> 5. Layer를 Ground 로 설정 (없으면 Add Layer에서 먼저 추가).

---

## Step 6 — 타일 배치

### 수동 모드

> 1. Window → 2D → Tile Palette 열기.
> 2. 왼쪽 상단 드롭다운에서 사용할 Palette 선택.
> 3. 브러시 도구(B)로 타일 선택 → Scene 뷰에서 클릭/드래그해서 배치.
> 4. 지우개(D)로 잘못 놓은 타일 제거.
> 5. 넓은 영역은 Box Fill(U)로 채우기.
> 6. Ctrl+S 로 씬 저장.

---

## Step 7 — 검증

Unity 에디터에서 `Assets > Refresh` 또는 `Ctrl+R`로 변경사항을 반영한다.
`Console` 창에서 컴파일 오류를 확인하고, 테스트는 `Window > General > Test Runner`에서 실행한다.

체크리스트:
- [ ] Tile 에셋이 Assets/_Project/Arts/Tilesets/ 에 생성됨
- [ ] 타일맵에 타일이 정상 표시 (보라색 아님)
- [ ] Tilemap_Ground 레이어가 Ground
- [ ] CompositeCollider2D 있음 (이음새 걸림 방지)
- [ ] 플레이 모드에서 플레이어가 타일 위를 정상 이동
- [ ] 플랫폼 끝에서 떨어지기 가능

## 주의사항

- PPU가 기존 타일셋과 다르면 타일 크기가 안 맞는다. 반드시 16으로 통일
- 타일맵에 CompositeCollider2D 없으면 이음새에서 캐릭터가 걸린다
- 기존 타일셋을 같은 파일명으로 덮어쓰면 Tile 에셋이 자동 업데이트된다
- 배경용 타일맵은 별도 오브젝트로 분리 (Tilemap_Background, 콜라이더 없이)
