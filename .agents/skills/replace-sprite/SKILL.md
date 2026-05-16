---
name: replace-sprite
description: 스프라이트 바꿔줘, 그림 교체해줘, 이미지 변경해줘 요청 시 사용. 캐릭터, 배경, 타일, UI 등 기존 스프라이트를 새 이미지로 교체한다.
---

# 스프라이트 교체

스프라이트 교체 요청을 받으면 이 절차를 순서대로 실행한다.

## 전제 조건

- 새 이미지 파일(PNG)이 이미 프로젝트 폴더에 있어야 함
- 에디트 모드여야 함

## Step 0 — Unity 수동 미사용

Unity 수동는 제거되었으므로 수동 모드 절차를 따른다.
코드와 파일 작성은 Codex가 수행하고, Unity 에디터 조작은 사용자에게 단계별로 안내한다.

---

## Step 1 — 파일 배치 확인

새 이미지가 올바른 폴더에 있는지 확인한다:

| 용도 | 경로 |
|---|---|
| 캐릭터 | Assets/_Project/Arts/Sprites/Characters/ |
| 배경/타일 | Assets/_Project/Arts/Tilesets/ |
| VFX | Assets/_Project/Arts/VFX/ |
| UI | Assets/_Project/Arts/Sprites/UI/ |

Unity 에디터에서 `Assets > Refresh` 또는 `Ctrl+R`로 변경사항을 반영한다.
`Console` 창에서 컴파일 오류를 확인하고, 테스트는 `Window > General > Test Runner`에서 실행한다.

체크리스트:
- [ ] 씬에서 스프라이트가 정상 표시 (보라색 아님)
- [ ] 크기/비율이 기존과 동일 (PPU 16 확인)
- [ ] 애니메이션 정상 재생
- [ ] 콜라이더가 있다면 외형과 맞는지 확인

## 주의사항

- 같은 파일명으로 덮어쓰는 게 가장 안전 (Unity GUID 참조 유지)
- 파일명을 바꾸면 기존 참조가 끊긴다
- PPU가 기존과 다르면 크기가 달라진다. 반드시 16으로 맞출 것
- 시트 구조(프레임 수, 이름)가 바뀌면 애니메이션 클립 재세팅 필요
