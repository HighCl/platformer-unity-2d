---
name: replace-sprite
description: 스프라이트 바꿔줘, 그림 교체해줘, 이미지 변경해줘 요청 시 사용. 캐릭터, 배경, 타일, UI 등 기존 스프라이트를 새 이미지로 교체한다.
---

# 스프라이트 교체

스프라이트 교체 요청을 받으면 아래 절차를 따른다.

## 전제 조건

- 새 이미지 파일(PNG)이 프로젝트 폴더에 있어야 한다.
- Unity 에디터가 에디트 모드여야 한다.
- `.prefab`, `.unity`, `.anim`, `.controller`, `.asset` 파일은 텍스트로 직접 수정하지 않는다.

## Step 1 — 파일 배치 확인

새 이미지가 올바른 폴더에 있는지 확인한다.

| 용도 | 경로 |
|---|---|
| 캐릭터 | `Assets/_Project/Arts/Sprites/Characters/` |
| 배경/타일 | `Assets/_Project/Arts/Tilesets/` |
| VFX | `Assets/_Project/Arts/VFX/` |
| UI | `Assets/_Project/Arts/Sprites/UI/` |

파일이 없으면 사용자에게 실제 파일 경로를 요청한다.

## Step 2 — 임포트 설정 안내

사용자에게 안내한다.

> 1. Project 패널에서 새 이미지 파일 선택.
> 2. Inspector에서 `Texture Type`을 `Sprite (2D and UI)`로 설정.
> 3. 단일 이미지면 `Sprite Mode: Single`, 시트면 `Multiple` 선택.
> 4. `Pixels Per Unit`을 `16`으로 설정.
> 5. `Filter Mode`를 `Point (no filter)`로 설정.
> 6. `Compression`을 `None`으로 설정.
> 7. `Apply` 클릭.

## Step 3 — SpriteRenderer 교체 안내

씬 오브젝트 교체:

> 1. Hierarchy에서 대상 오브젝트 선택.
> 2. Inspector의 `SpriteRenderer` 컴포넌트 확인.
> 3. Sprite 슬롯에 새 스프라이트를 드래그.
> 4. 색상이 보라색이면 Material을 `Sprites/Default`로 변경.
> 5. `Ctrl+S`로 씬 저장.

프리팹 교체:

> 1. Project 패널에서 대상 프리팹 더블클릭.
> 2. Prefab 편집 모드에서 루트 오브젝트 선택.
> 3. `SpriteRenderer`의 Sprite 슬롯에 새 스프라이트 연결.
> 4. 뒤로 가기 버튼으로 나가며 저장.

## Step 4 — 애니메이션 클립 교체 안내

스프라이트가 애니메이션에서 사용되면 Animation 창에서 교체한다.

> 1. Project 패널에서 `.anim` 클립 선택.
> 2. `Window > Animation > Animation` 창 열기.
> 3. 대상 프레임 키를 선택.
> 4. 새 스프라이트를 해당 프레임에 드래그.
> 5. 모든 프레임 교체 후 저장.

## Step 5 — Tile 에셋 교체 안내

타일 스프라이트 교체:

> 1. Project 패널에서 대상 Tile 에셋 선택.
> 2. Inspector의 Sprite 슬롯에 새 스프라이트 드래그.
> 3. 이미 씬에 배치된 타일은 자동 반영된다.

## Step 6 — 검증

- Unity 에디터에서 `Assets > Refresh` 또는 `Ctrl+R` 실행
- Console 창에서 컴파일 에러 0개 확인
- 씬에서 스프라이트가 정상 표시됨
- 보라색 머티리얼 문제가 없음
- 크기와 비율이 기존과 맞음
- 애니메이션이 정상 재생됨
- 콜라이더가 외형과 크게 어긋나지 않음

## 주의사항

- 같은 파일명으로 덮어쓰는 방식이 GUID 참조를 유지하므로 가장 안전하다.
- 파일명을 바꾸면 기존 참조가 끊길 수 있다.
- PPU가 다르면 크기가 달라진다. 이 프로젝트 기준은 16이다.
- 시트 구조가 바뀌면 애니메이션 클립을 다시 설정해야 한다.
