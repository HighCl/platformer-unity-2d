---
topic: Unity Project Configuration
type: codebase
last_compiled: 2026-05-16
source_count: 8
status: active
---

# Unity Project Configuration

## Purpose [coverage: medium -- 8 sources]
Unity 프로젝트 설정 토픽은 Unity 버전, 패키지 의존성, 어셈블리 정의, 테스트 러너 연결을 설명한다. 현재 변경분에서는 `ProjectSettings/ProjectVersion.txt`가 `6000.3.11f1`에서 `6000.3.9f1`로 바뀐 점이 중요하다.

## Architecture [coverage: medium -- 8 sources]
프로젝트는 `Packages/manifest.json`으로 Unity 패키지를 관리하고, 각 코드 영역은 `.asmdef`로 분리된다.

주요 설정:
- Unity Editor: `6000.3.9f1`
- URP: `com.unity.render-pipelines.universal` 17.3.0
- Input System: `com.unity.inputsystem` 1.19.0
- Unity Test Framework: `com.unity.test-framework` 1.6.0

## Talks To [coverage: medium -- 5 sources]
- **Assembly Definition**: 런타임과 테스트 어셈블리가 명시적 참조를 가진다.
- **Unity Test Runner**: 테스트 어셈블리는 `UnityEngine.TestRunner`, `UnityEditor.TestRunner`, `nunit.framework.dll`을 참조한다.

## API Surface [coverage: low -- 1 source]
설정 파일 자체는 런타임 API를 노출하지 않는다. 다만 `.asmdef`의 `references` 필드가 코드에서 접근 가능한 어셈블리 경계를 결정한다.

## Data [coverage: medium -- 5 sources]
`ProjectVersion.txt`는 현재 에디터 버전과 리비전을 저장한다. `manifest.json`은 패키지 버전을 고정하고, `packages-lock.json`은 해석된 의존성 잠금을 제공한다.

## Key Decisions [coverage: medium -- 5 sources]
- **명시적 어셈블리 경계**: 모든 주요 코드 영역에 `.asmdef`를 두어 참조 가능 범위를 제한한다.
- **Unity 기본 패키지 중심 구성**: 패키지는 Unity 공식 레지스트리와 프로젝트 필수 2D/테스트 의존성 중심으로 유지한다.
- **Input System 사용**: 레거시 입력 대신 생성된 Input System 래퍼를 사용한다.

## Gotchas [coverage: high -- 8 sources]
- README의 "Unity 6.0.3" 설명과 실제 `ProjectVersion.txt`의 `6000.3.9f1`은 표현 수준이 다르다. 정확한 에디터 버전이 필요하면 `ProjectVersion.txt`를 우선 확인한다.
- `ProjectSettings/EditorBuildSettings.asset`, `ProjectSettings/ProjectSettings.asset`, `ShaderGraphSettings.asset`는 변경되어 있지만 Unity 직렬화 파일이므로 위키 컴파일에서 직접 해석하지 않았다.
- `.asmdef`의 `autoReferenced`가 false라 참조 누락 시 타입을 찾지 못할 수 있다.

## Sources
- [ProjectSettings/ProjectVersion.txt](../../ProjectSettings/ProjectVersion.txt)
- [Packages/manifest.json](../../Packages/manifest.json)
- [Packages/packages-lock.json](../../Packages/packages-lock.json)
- [Assets/_Project/Scripts/Data/Platformer.Data.asmdef](../../Assets/_Project/Scripts/Data/Platformer.Data.asmdef)
- [Assets/_Project/Scripts/Core/Platformer.Core.asmdef](../../Assets/_Project/Scripts/Core/Platformer.Core.asmdef)
- [Assets/_Project/Scripts/Game/Platformer.Game.asmdef](../../Assets/_Project/Scripts/Game/Platformer.Game.asmdef)
- [Assets/_Project/Scripts/UI/Platformer.UI.asmdef](../../Assets/_Project/Scripts/UI/Platformer.UI.asmdef)
- [Assets/_Project/Scripts/Core/PlayModeTests/Platformer.Core.PlayModeTests.asmdef](../../Assets/_Project/Scripts/Core/PlayModeTests/Platformer.Core.PlayModeTests.asmdef)
