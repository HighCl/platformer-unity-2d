---
name: apply-tileset
description: 타일셋 스프라이트 시트(PNG)를 임포트하고 슬라이싱, Tile 에셋 생성, 타일맵 적용까지 처리한다. "타일셋 올렸어, 적용해줘" 요청 시 사용한다.
---

# 타일셋 적용

디자이너가 타일셋 시트 PNG를 넣으면, Codex는 파일 배치와 검증 기준을 정리하고 Unity 에디터 조작은 단계별로 안내한다.

## 전제 조건

- PNG 파일이 `Assets/_Project/Arts/Tilesets/` 또는 외부 에셋팩 폴더에 있어야 한다.
- Unity 에디터가 에디트 모드여야 한다.
- `.unity`, `.prefab`, `.asset` 파일은 텍스트로 직접 수정하지 않는다.

## Step 1 — 임포트 설정

사용자에게 안내한다.

> 1. Project 패널에서 타일셋 PNG 선택.
> 2. Inspector에서 `Texture Type`을 `Sprite (2D and UI)`로 변경.
> 3. `Sprite Mode`를 `Multiple`로 변경.
> 4. `Pixels Per Unit`을 `16`으로 설정.
> 5. `Filter Mode`를 `Point (no filter)`로 설정.
> 6. `Compression`을 `None`으로 설정.
> 7. `Apply` 클릭.

## Step 2 — 슬라이싱

> 1. Project 패널에서 타일셋 PNG 선택.
> 2. Inspector 하단 `Sprite Editor` 클릭.
> 3. `Slice` 클릭.
> 4. `Type: Grid By Cell Size` 선택.
> 5. 셀 크기를 `16 x 16`으로 입력. 실제 타일 크기가 다르면 그 값 사용.
> 6. `Slice` 클릭 후 `Apply`.

## Step 3 — 스프라이트 이름 확인

> Sprite Editor에서 각 슬라이스 이름을 확인한다.

권장 이름 패턴:

- `Top`, `Mid`, `Bot`
- `Left`, `Right`
- `Single`
- `SlopeLeft`, `SlopeRight`

## Step 4 — Tile 에셋 생성

Tile Palette 사용을 안내한다.

> 1. `Window > 2D > Tile Palette` 열기.
> 2. `Create New Palette` 클릭.
> 3. 팔레트 이름 지정 후 생성.
> 4. 슬라이스된 스프라이트를 Tile Palette 창으로 드래그.
> 5. 저장 위치를 `Assets/_Project/Arts/Tilesets/`로 지정.
> 6. Tile 에셋이 자동 생성되었는지 확인.

Collider 기준:

- 일반 바닥: Grid
- 경사면: Sprite
- 배경 장식: None

## Step 5 — 타일맵 오브젝트 준비

사용자에게 안내한다.

> 1. 씬에 Grid가 없으면 Hierarchy 우클릭 → `2D Object > Tilemap > Rectangular`.
> 2. Tilemap 오브젝트 이름을 `Tilemap_Ground`로 변경.
> 3. Inspector에서 `TilemapCollider2D` 추가.
> 4. `TilemapCollider2D`의 Composite Operation을 `Merge`로 설정.
> 5. `Rigidbody2D` 추가 후 Body Type을 `Static`으로 설정.
> 6. `CompositeCollider2D` 추가.
> 7. Layer를 `Ground`로 설정.

## Step 6 — 타일 배치

사용자에게 안내한다.

> 1. `Window > 2D > Tile Palette` 열기.
> 2. 사용할 Palette 선택.
> 3. 브러시 도구로 타일 선택.
> 4. Scene 뷰에서 클릭 또는 드래그로 배치.
> 5. 넓은 영역은 Box Fill 도구 사용.
> 6. 잘못 놓은 타일은 지우개 도구로 제거.
> 7. `Ctrl+S`로 씬 저장.

## Step 7 — 검증

- Unity 에디터에서 `Assets > Refresh` 또는 `Ctrl+R` 실행
- Console 창에서 컴파일 에러 0개 확인
- Tile 에셋이 `Assets/_Project/Arts/Tilesets/`에 생성됨
- 타일맵에 타일이 정상 표시됨
- `Tilemap_Ground` 레이어가 `Ground`
- `CompositeCollider2D`가 있음
- 플레이 모드에서 플레이어가 타일 위를 정상 이동

## 주의사항

- PPU는 기존 타일셋과 맞춰야 한다. 이 프로젝트 기준은 16이다.
- `CompositeCollider2D`가 없으면 타일 이음새에 걸릴 수 있다.
- 배경 타일맵은 `Tilemap_Background`처럼 별도 오브젝트로 분리하고 콜라이더를 두지 않는다.
