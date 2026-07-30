---
name: unity-urp-dev
description: |
  Unity URP RenderGraph 기반 RendererFeature 개발 시 반드시 참조. 프로젝트의 URP 버전을 자동 감지하여 해당 버전의 공식 GitHub/Unity Docs를 참조하게 안내한다. RenderGraph API 패턴, 모바일 최적화, RendererFeature 구조를 다룬다. "URP", "RenderGraph", "RendererFeature", "ScriptableRenderPass", "PostProcess", "shader", "HLSL", "DoF", "SSR", "Bloom", "TAA" 등 URP 렌더링 기능 개발 시 반드시 사용.
---

# Unity URP Dev Skill

## 섹션 1: URP 버전 감지 (필수 첫 단계)

이 스킬을 사용할 때 **반드시** 가장 먼저 아래 단계를 수행한다.

1. `Packages/packages-lock.json` 또는 `Packages/manifest.json`을 읽어 `com.unity.render-pipelines.universal` 버전을 확인한다.
2. 버전 문자열에서 `{major}.{minor}` 를 추출한다.
   - 예: `"17.3.0"` → `17.3`, `"14.0.11"` → `14.0`
3. 추출한 버전으로 섹션 3의 URL 패턴을 구성하여 해당 버전의 공식 문서를 참조한다.
4. 버전 감지 없이 코드를 작성하지 않는다. API는 버전마다 다르다.

## 섹션 2: 로컬 레퍼런스 파일 활용 (코드 작성 전 필독)

이 스킬 디렉토리에는 실전 구현 패턴이 담긴 레퍼런스 파일이 있다. **코드를 작성하기 전에 아래 라우팅 테이블을 보고 관련 파일을 읽어라.** 로컬 레퍼런스는 검증된 패턴과 예제를 즉시 제공하므로 온라인 문서보다 우선적으로 활용한다. 온라인 문서(섹션 3)는 최신 API 시그니처 확인이나 로컬 레퍼런스에 없는 내용을 검증할 때 보조적으로 사용한다.

레퍼런스 파일 경로: `~/.claude/skills/unity-urp-dev/references/`

### 작업 유형별 레퍼런스 라우팅

| 작업 유형 | 읽어야 할 파일 |
|---|---|
| RenderGraph 패스 구조, PassData, `AddRasterRenderPass`, `AddUnsafePass`, `AddComputePass` 기본 | `01_URP_Core_Package_Guide.md` |
| Volume System, `VolumeComponent`, `IPostProcessComponent`, 커스텀 볼륨 파라미터 | `02_RenderPipelineCore_Guide.md` |
| Bloom, DoF, Motion Blur, Color Grading, Lens Distortion, TAA, SSAO/SSGI 구현 | `03_PostProcessing_Effects_Guide.md` |
| HLSL 셰이더 작성, ShaderLibrary `#include`, 수학 함수, 샘플링/필터링, 색공간 변환 | `04_ShaderLibrary_Guide.md` |
| HLSL 흐름 제어 최적화 (`[loop]`/`[unroll]`, `[branch]`/`[flatten]`), 모바일 GPU attribute 선택 | `04_ShaderLibrary_Guide.md` (HLSL 흐름 제어 최적화 Attribute 섹션) |
| `Unity.Mathematics` (float3 등), Burst 컴파일러, Job System, NativeArray | `05_Mathematics_Collections_Guide.md` |
| Unity 6.0 신기능, GPU Resident Drawer, RenderGraph Viewer 디버깅, XR/VR 지원 | `06_Unity60_NewFeatures_Guide.md` |
| Constant Buffer, `NativeContainer`, unsafe 포인터, 제로 카피 전송, GPU 메모리 관리 | `07_Advanced_Memory_Management_Guide.md` |
| Compute Shader, Compute Pass, GPU Culling, `AsyncGPUReadback`, 멀티패스 컴퓨트 | `08_Compute_Shader_Integration_Guide.md` |

### 복합 작업 시 읽기 순서

- 커스텀 포스트프로세싱 이펙트 신규 개발: `01` → `02` → `03` (해당 이펙트 섹션)
- 셰이더 포함 포스트프로세싱: `01` → `04`
- 고성능 Compute 기반 이펙트: `08` → `07` (메모리 관리 필요 시)
- Unity 6.0 신기능 활용: `06` → 관련 기능 파일

각 파일은 목차(Table of Contents)를 포함하므로, 전체를 읽지 않고 필요한 섹션만 offset/limit으로 읽어도 된다.

## 섹션 3: 온라인 문서 참조 URL 패턴

로컬 레퍼런스로 충분하지 않을 때, 또는 특정 버전의 API 시그니처를 검증할 때 사용한다.

### GitHub Core Docs (RenderGraph, 렌더 패스 내부 구조)

Unity Graphics 레포지토리는 **버전 태그를 별도로 제공하지 않으며** `master` 브랜치가 최신 소스를 포함한다. 특정 버전 코드 참조는 로컬 `Library/PackageCache`를 우선 사용한다.

```
# 최신 문서 (master)
https://github.com/Unity-Technologies/Graphics/tree/master/Packages/com.unity.render-pipelines.core/Documentation~

# URP 소스 (master)
https://github.com/Unity-Technologies/Graphics/tree/master/Packages/com.unity.render-pipelines.universal/Runtime/
```

- **권장**: 버전 정확도가 필요하면 GitHub 대신 `Library/PackageCache/com.unity.render-pipelines.core@{full-version}/` 로컬 소스를 직접 읽는다.
- GitHub 문서는 개념/구조 파악용, 정확한 API 시그니처는 Unity 공식 API Docs나 로컬 캐시를 사용한다.

### Unity 공식 API Docs (클래스/메서드 레퍼런스)

```
https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@{major}.{minor}/api/index.html
```

- 예: 버전 `17.3` →
  `https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@17.3/api/index.html`

### URP Manual (개념 설명 및 사용 가이드)

```
https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@{major}.{minor}/manual/index.html
```

### 로컬 소스 참조 (오프라인 또는 빠른 확인)

프로젝트 로컬에 캐시된 패키지 소스를 직접 읽을 수 있다.

```
Library/PackageCache/com.unity.render-pipelines.universal@{full-version}/
Library/PackageCache/com.unity.render-pipelines.core@{full-version}/
```

## 섹션 4: RenderGraph 기반 RendererFeature 개발 가이드

> 자세한 패턴과 완전한 예제는 `01_URP_Core_Package_Guide.md`를 읽어라.

URP 17+ 에서는 레거시 `OnCameraRender` / `Configure` / `Execute` 방식 대신 **RenderGraph API**를 사용한다.

### 기본 클래스 구조

```csharp
// ScriptableRendererFeature 파생
public class MyFeature : ScriptableRendererFeature
{
    public override void Create() { /* 패스 인스턴스 생성 */ }
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) { /* 패스 등록 */ }
}

// ScriptableRenderPass 파생
public class MyRenderPass : ScriptableRenderPass
{
    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData) { /* RenderGraph 구성 */ }
}
```

### PassData 규칙

- **class**로 정의 (struct 사용 시 컴파일 오류)
- 모든 필드는 `internal` 접근자 사용
- 논리적 그룹별로 필드 정렬: Setup → Input → Material → Temp → Output

### RenderGraph API 선택 기준

세 가지 패스 API는 목적에 따라 선택한다. 잘못 선택하면 RenderGraph 자동 최적화가 무력화되거나 컴파일 오류가 발생한다.

**판단 체크리스트**

```
1. Compute Shader를 dispatch 하는가?
   → YES: AddComputePass

2. 아래 중 하나라도 해당하는가?
   - 단일 패스 내에서 렌더 타겟을 동적으로 전환
   - cmd.SetRenderTarget() 직접 제어 필요
   - cmd.CopyTexture(), cmd.Blit() 등 CommandBuffer API 직접 사용
   - 복수 렌더 타겟을 MRT로 바인딩
   → YES: AddUnsafePass

3. 그 외 (픽셀 셰이더 기반, 단일 렌더 타겟):
   → AddRasterRenderPass  ← 가능하면 항상 이것을 선택
```

| 항목 | AddRasterRenderPass | AddUnsafePass | AddComputePass |
|---|---|---|---|
| 렌더 타겟 설정 | `SetRenderAttachment`로 RecordRenderGraph 시점 선언 | `cmd.SetRenderTarget()` 런타임 직접 제어 | 없음 |
| CommandBuffer 접근 | 제한된 `RasterCommandBuffer` | 전체 `CommandBuffer` API | `ComputeCommandBuffer` |
| RenderGraph 의존성 추적 | 완전 추적 | 추적 불가 | 완전 추적 |
| 패스 머징 / 타일 최적화 | ✅ 자동 적용 | ❌ 직렬화 강제 | N/A |
| 권장 우선순위 | 1순위 (기본 선택) | 불가피한 경우만 | Compute 전용 |

> `AddUnsafePass`는 레거시 CommandBuffer 패스를 RenderGraph로 마이그레이션할 때 중간 단계로도 적합하다. 이후 시간이 생기면 `AddRasterRenderPass`로 승격을 권장한다.

### 핵심 패턴

**frameData에서 리소스 접근**
```csharp
var resourceData = frameData.Get<UniversalResourceData>();
TextureHandle cameraColor = resourceData.activeColorTexture;
TextureHandle cameraDepth = resourceData.activeDepthTexture;
```

**AddRasterRenderPass** — 단일 렌더 타겟, 픽셀 셰이더 기반 패스의 표준
```csharp
using (var builder = renderGraph.AddRasterRenderPass<PassData>("MyPass", out var passData))
{
    builder.SetRenderAttachment(outputHandle, 0, AccessFlags.WriteAll); // DontCare loadOp
    builder.UseTexture(inputHandle, AccessFlags.Read);
    builder.SetRenderFunc(static (PassData data, RasterGraphContext ctx) => { /* 실행 */ });
}
```

**AddUnsafePass** — CommandBuffer 직접 접근이 불가피한 경우
```csharp
using (var builder = renderGraph.AddUnsafePass<PassData>("MyPass", out var passData))
{
    builder.UseTexture(handle, AccessFlags.ReadWrite);
    builder.SetRenderFunc(static (PassData data, UnsafeGraphContext ctx) =>
    {
        CommandBuffer cmd = CommandBufferHelpers.GetNativeCommandBuffer(ctx.cmd);
        cmd.CopyTexture(data.src, data.dst); // CopyTexture 등 직접 CommandBuffer API
    });
}
```

**AllowPassCulling / AllowGlobalStateModification**
```csharp
builder.AllowPassCulling(false);            // 카메라 렌더링 중 컬링 방지
builder.AllowGlobalStateModification(true); // 글로벌 텍스처 Set 허용 시
```

### URP 17+ 이전 버전 호환 (선택)

RenderGraph를 지원하지 않는 환경을 위해 `RecordRenderGraph` 외에 `OnCameraRender`을 병행 구현할 수 있으나, 17+ 타겟 프로젝트에서는 RenderGraph 단독 사용을 권장한다.

## 섹션 5: 모바일(Vulkan 이상) 최적화 체크리스트

### 타일 메모리 최적화

- `RenderBufferLoadAction.DontCare` — 타일 메모리 초기화 비용 제거. 이전 내용이 필요 없는 렌더 타겟에 적용.
- `RenderBufferStoreAction.DontCare` — 임시 버퍼를 메인 메모리로 저장할 필요 없을 때 적용.
- `RenderBufferStoreAction.StoreAndResolve` — MSAA resolve가 필요한 경우.

### HLSL Precision

- `half` precision 적극 사용: `half4`, `half3`, `half2`, `half`
- 월드 포지션·뎁스·UV 등 정밀도가 중요한 값에만 `float` 사용
- 컬러 등, 정밀도가 중요하지 않는 값들은 `half`로 충분

### 플랫폼 분기 매크로

```hlsl
// Depth 방향 차이
#if UNITY_REVERSED_Z
    // DX11/12, Metal, Vulkan: depth 1.0 near, 0.0 far
#else
    // OpenGL, OpenGL ES: depth 0.0 near, 1.0 far
#endif

// UV 기원 차이
// Vulkan은 NDC y축이 위→아래이므로 Unity가 UNITY_UV_STARTS_AT_TOP을 1로 설정한다
#if UNITY_UV_STARTS_AT_TOP
    // DX11/12, Metal, Vulkan: UV (0,0) 좌상단
#else
    // OpenGL, OpenGL ES: UV (0,0) 좌하단
#endif
```

### HLSL 흐름 제어 Attribute

반복문과 조건문에는 GPU 컴파일러 동작을 제어하는 attribute가 있다. **상황에 맞지 않는 attribute는 오히려 성능 저하를 유발한다.**

**반복문: `[loop]` vs `[unroll]`**

| 조건 | 권장 | 이유 |
|---|---|---|
| 반복 횟수가 런타임 변수 (`_SampleCount` 등) | `[loop]` (= `UNITY_LOOP`) | 컴파일 타임 전개 불가 |
| 반복 횟수가 많음 (≥ 8~16) | `[loop]` | 코드 팽창 방지 (Mali 명령어 캐시 압박) |
| 반복 횟수가 컴파일 상수이고 적음 (≤ 8) | `[unroll]` | 분기 제거로 ALU 파이프라인 효율화 |

> `UNITY_LOOP` 는 HLSL 경로에서 `[loop]` 로 매핑된다. 반복 횟수가 상수인 소규모 루프에는 오히려 `[unroll]`이 더 적합할 수 있다.

> 주의: `[loop]` 내부에서는 암묵적 gradient를 사용하는 `SAMPLE_TEXTURE2D()` 사용 불가. `SAMPLE_TEXTURE2D_LOD()` 또는 `SAMPLE_TEXTURE2D_GRAD()` 로 대체한다.

```hlsl
// 런타임 변수 반복 → [loop]
UNITY_LOOP
for (int i = 0; i < (int)_SampleCount; i++)
{
    color += SAMPLE_TEXTURE2D_LOD(_Tex, sampler_Tex, uv, 0); // LOD 명시
}

// 컴파일 상수 소규모 반복 → [unroll]
[unroll]
for (int i = 0; i < 4; i++)
{
    color += SAMPLE_TEXTURE2D(_Tex, sampler_Tex, offsets[i]); // gradient OK
}
```

**조건문: `[branch]` vs `[flatten]`**

| 조건 | 권장 | 이유 |
|---|---|---|
| 조건값이 draw call 전체에서 동일 (uniform) | `[branch]` | 비용 큰 블록 완전 스킵 가능 |
| 조건이 타일 메모리 연산(framebuffer fetch)을 포함 | `[branch]` | `[flatten]`은 false여도 블록 실행 → fetch 강제 발생 |
| 조건값이 픽셀마다 다름 (텍스처 샘플링 결과 등) | `[flatten]` | wave 분기 오버헤드 방지 |
| 조건 평가 비용 < 분기 선택 블록 비용 | `[flatten]` | 양쪽 다 실행해도 분기보다 저렴 |

```hlsl
// uniform 조건 → [branch]: false 경로 완전 스킵
UNITY_BRANCH
if (_FeatureEnabled > 0.5)
{
    color = ExpensiveEffect(color); // _FeatureEnabled가 draw call 단위로 고정
}

// 픽셀 단위 발산 조건 → [flatten]: wave 분기 방지
UNITY_FLATTEN
if (roughness < _ThresholdValue)
{
    color = BlurSample(uv); // roughness는 픽셀마다 다름
}
```

> `UNITY_BRANCH` = `[branch]`, `UNITY_FLATTEN` = `[flatten]` (HLSLSupport.cginc 매핑)

### 셰이더 컴파일 최적화

> ⚠️ `#pragma prefer_hlslcc gles`와 `#pragma exclude_renderers d3d11_9x`는 Unity 6.0에서 의미 없다. Unity 2021 이후 모든 플랫폼이 HLSLcc를 기본 사용하며, DX11 9.x feature level 지원은 Unity 6.0에서 제거되었다. 이 pragma들을 추가하지 않는다.

### Overdraw 방지

- 초기 코스 패스(depth prepass 등)로 오클루전 마스킹 후 파인 패스 진행
- 불투명 오브젝트는 front-to-back 렌더링 순서 유지
- 반투명 오브젝트 수 최소화

## 섹션 5.5: 알려진 함정 (Known Pitfalls)

코드 작성 전 반드시 확인한다. 이 함정들은 런타임 오류나 시각적 버그로 이어진다.

### 1. Blitter API와 셰이더 텍스처 이름 불일치 ⚠️

`Blitter.BlitTexture` / `Blitter.BlitCameraTexture`는 소스 텍스처를 **`_BlitTexture`** 이름으로 Material에 바인딩한다.
셰이더에서 `_MainTex`로 선언하면 흰색 기본 텍스처를 샘플링하여 화면이 하얗게 된다.

```hlsl
// ❌ 잘못된 패턴 — Blitter를 쓰면서 _MainTex 선언
TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);
// → Blitter가 바인딩한 소스를 읽지 못함

// ✅ 올바른 패턴 A — Blitter 호환 직접 선언
TEXTURE2D_X(_BlitTexture);
SAMPLER(sampler_BlitTexture);
// SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, uv)

// ✅ 올바른 패턴 B — Blit.hlsl include (선언 포함)
#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
// Blit.hlsl이 _BlitTexture, sampler_BlitTexture, Varyings, Vert를 모두 선언함
```

**예외**: `AddUnsafePass` 내부에서 `cmd.SetGlobalTexture("_MainTex", ...)` 로 직접 바인딩하는 경우는 `_MainTex` 사용 가능.

### 2. `PostProcessPass.GetCompatibleDescriptor` 는 internal API ⚠️

URP 소스 내부의 `internal static` 메서드로, 외부 프로젝트에서 호출하면 **컴파일 오류**가 발생한다.

```csharp
// ❌ 컴파일 오류 — internal API
var desc = PostProcessPass.GetCompatibleDescriptor(
    cameraData.cameraTargetDescriptor, width, height, format, GraphicsFormat.None);

// ✅ 대체 패턴 — descriptor 직접 구성
var desc = cameraData.cameraTargetDescriptor;
desc.width            = width;
desc.height           = height;
desc.graphicsFormat   = format;
desc.depthBufferBits  = 0;   // 포스트프로세스 텍스처는 depth buffer 불필요
desc.msaaSamples      = 1;   // MSAA 비활성화
```

### 3. HLSL CBUFFER에서 int 타입 미지원 ⚠️

SRP CBUFFER(`CBUFFER_START`) 안에 `int` 타입을 선언하면 일부 플랫폼에서 값이 올바르게 전달되지 않는다.
샘플 수 등 정수 파라미터는 `float`으로 선언하고, C# 쪽도 `SetFloat`으로 전달한다.

```hlsl
// ❌ CBUFFER 내 int 선언 — 플랫폼 비호환
CBUFFER_START(UnityPerMaterial)
    int _SampleCount;
CBUFFER_END

// ✅ float으로 선언 후 캐스팅
CBUFFER_START(UnityPerMaterial)
    float _SampleCount;  // C#: material.SetFloat("_SampleCount", (float)count)
CBUFFER_END
int sampleCount = (int)_SampleCount;  // 셰이더 내부에서 캐스팅
```

---

## 섹션 6: 개발 워크플로우

1. **버전 감지**: `Packages/packages-lock.json`에서 URP 버전 읽기
2. **레퍼런스 파일 읽기**: 섹션 2 라우팅 테이블에서 작업에 맞는 파일 선택 후 읽기
3. **문서 확인** (필요 시): 섹션 3의 URL로 해당 버전 API 스펙 검증
4. **로컬 소스 참조** (필요 시): `Library/PackageCache/com.unity.render-pipelines.universal@{version}/`
5. **구현**: 레퍼런스 파일의 패턴 적용, 섹션 4 기본 구조 활용

### 자주 참조하는 URP 클래스/네임스페이스

| 클래스 | 네임스페이스 | 용도 |
|---|---|---|
| `ScriptableRendererFeature` | `UnityEngine.Rendering.Universal` | 피처 베이스 클래스 |
| `ScriptableRenderPass` | `UnityEngine.Rendering.Universal` | 패스 베이스 클래스 |
| `UniversalResourceData` | `UnityEngine.Rendering.Universal` | 카메라 리소스 접근 |
| `UniversalRenderingData` | `UnityEngine.Rendering.Universal` | 렌더링 데이터 접근 |
| `RenderGraph` | `UnityEngine.Rendering.RenderGraphModule` | RenderGraph 빌더 |
| `TextureHandle` | `UnityEngine.Rendering.RenderGraphModule` | RenderGraph 텍스처 핸들 |
| `ContextContainer` | `UnityEngine.Rendering` | frameData 컨테이너 |
