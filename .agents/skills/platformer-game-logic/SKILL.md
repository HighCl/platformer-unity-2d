---
name: platformer-game-logic
description: Platformer Unity 2D 프로젝트의 Core/Game 코드에 적용하는 게임 로직 안전 규칙. PlayerState 상태 관리, FixedUpdate 물리 처리, OverlapCircle 지면 감지, CapsuleCollider2D/NoFriction/CompositeCollider2D 2D 플랫폼 물리 패턴, 입력 x축 처리, URP 2D 스프라이트 머티리얼, SpriteRenderer.flipX, 적 추적 AI 데드존, 스톰프 판정, 사망 처리, 새 기능 스펙 확정, 코루틴 캐싱을 다룰 때 사용한다.
---

# 게임 로직 규칙

## PlayerState enum

플레이어 상태는 `Platformer.Data.PlayerState` enum으로 관리한다. 조건문 분기를 무분별하게 늘리지 않는다.

```csharp
private PlayerState _state = PlayerState.Idle;

private void _SetState(PlayerState newState)
{
    if (_state == newState)
        return;

    _state = newState;
}
```

## 물리는 FixedUpdate

`Rigidbody2D` 조작은 `FixedUpdate`에서 수행한다. `Update`에서는 입력만 받고 플래그로 저장한다.

```csharp
void Update()
{
    if (_input.Player.Jump.WasPressedThisFrame())
        _isJumpRequested = true;
}

void FixedUpdate()
{
    _rb.linearVelocity = new Vector2(_moveInput * _settings.moveSpeed, _rb.linearVelocity.y);

    if (_isJumpRequested)
    {
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _settings.jumpForce);
        _isJumpRequested = false;
    }
}
```

## 지면 감지

지면 감지는 Raycast보다 `OverlapCircle`을 우선 사용한다.

```csharp
[SerializeField] private Transform _groundCheck;
[SerializeField] private float _groundCheckRadius = 0.1f;
[SerializeField] private LayerMask _groundLayer;

private bool _IsGrounded()
    => Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _groundLayer);
```

- `groundCheck`는 발 위치에 빈 GameObject로 둔다.
- Layer는 Inspector에서 Ground 레이어로 설정한다.

## 2D 플랫포머 물리 패턴

### 플레이어 콜라이더

- 플레이어에는 `BoxCollider2D` 대신 `CapsuleCollider2D` Vertical을 사용한다.
- 박스 콜라이더는 타일 이음새에서 모서리가 걸릴 수 있다.

```csharp
var capsule = player.AddComponent<CapsuleCollider2D>();
capsule.direction = CapsuleDirection2D.Vertical;
```

### NoFriction PhysicsMaterial2D

- 플레이어 콜라이더에는 `Assets/_Project/Datas/NoFriction.physicsMaterial2D`를 적용한다.
- 마찰이 있으면 벽면에 달라붙을 수 있다.

### 타일맵 CompositeCollider2D

Tilemap에는 `TilemapCollider2D`만 두지 않는다.

```text
Tilemap_Ground:
  - Rigidbody2D (Static)
  - TilemapCollider2D (compositeOperation = Merge)
  - CompositeCollider2D
```

### Ground 레이어 체크

- `ProjectSettings/TagManager.asset`에 `Ground` 레이어가 있어야 한다.
- 타일맵 오브젝트의 Layer가 `Ground`여야 한다.
- `PlayerController`의 `_groundLayer`에 Ground가 연결되어야 한다.

## 입력과 이동속도

2D 플랫포머 이동은 x축만 사용한다. y축이 섞이면 대각선 정규화로 이동속도가 감소한다.

```csharp
var raw = _input.Player.Move.ReadValue<Vector2>();
_moveInput = raw.x != 0f ? Mathf.Sign(raw.x) : 0f;
```

## URP 2D 스프라이트 머티리얼

스프라이트가 보라색이면 `Sprites/Default` 머티리얼을 적용한다.

```csharp
sr.sharedMaterial = new Material(Shader.Find("Sprites/Default"));
```

## 스프라이트 좌우 반전

플레이어 좌우 반전은 `transform.localScale.x` 대신 `SpriteRenderer.flipX`를 사용한다. 자식 오브젝트 위치가 뒤집히면 GroundCheck 등이 깨질 수 있다.

```csharp
_spriteRenderer.flipX = _moveDirection < 0f;
```

적 캐릭터는 기본 방향에 따라 조건을 조정한다.

```csharp
// 기본 방향이 오른쪽이면
_spriteRenderer.flipX = directionX < 0f;

// 기본 방향이 왼쪽이면
_spriteRenderer.flipX = directionX > 0f;
```

## 적 추적 AI 데드존

추적형 적은 타겟 위치 근처에서 좌우 진동이 생기지 않도록 `stopDistance` 데드존을 둔다.

```csharp
[SerializeField] private float _stopDistanceX = 0.2f;

void FixedUpdate()
{
    var toTarget = _target.position - transform.position;
    if (Mathf.Abs(toTarget.x) <= _stopDistanceX)
    {
        _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
        return;
    }
}
```

## 스톰프 판정

밟기 판정은 충돌 노멀(`contact.normal`)을 우선 사용한다. `bounds` 비교는 보조로만 사용한다.

```csharp
foreach (var contact in collision.contacts)
{
    if (contact.normal.y >= 0.5f)
        return true;
}

var playerBottom = otherCollider.bounds.min.y;
var enemyTop = _collider.bounds.max.y;
return playerBottom >= enemyTop - _stompContactTolerance;
```

필요하면 적 머리 위에 전용 `EnemyStompZone` 트리거 콜라이더를 배치한다.

## 사망 처리

`Rigidbody2D.simulated = false`는 Animator 업데이트까지 막을 수 있으므로 사망 애니메이션에는 사용하지 않는다.

```csharp
private void _Die()
{
    _isDead = true;
    _rb.linearVelocity = Vector2.zero;
    _rb.gravityScale = 0f;
    _rb.constraints = RigidbodyConstraints2D.FreezeAll;

    if (_animator != null)
        _animator.SetTrigger("Die");

    Invoke(nameof(_ReloadScene), _deathReloadDelay);
}
```

## 새 기능 스펙 확정

비개발자가 "적 만들어줘"처럼 열린 요청을 하면 구현 전 아래를 확정한다.

1. 타입: 근접/원거리/고정
2. 감지 방식: 거리/시야/항상
3. 공격 방식: 접촉/투사체
4. 체력: 즉사/다단
5. 사망 처리: 파괴/애니메이션
6. 리스폰: 씬 리로드/타이머

확정 없이 구현하지 않는다. 사용자가 "알아서 해"라고 하면 기본값을 제안하고 승인 후 진행한다.

## 코루틴 캐싱

같은 코루틴을 여러 번 시작하지 않도록 레퍼런스를 저장하고 관리한다.

```csharp
private Coroutine _knockbackCoroutine;

private void _TakeKnockback(Vector2 force)
{
    if (_knockbackCoroutine != null)
        StopCoroutine(_knockbackCoroutine);

    _knockbackCoroutine = StartCoroutine(_CoKnockback(force));
}

private IEnumerator _CoKnockback(Vector2 force)
{
    _rb.AddForce(force, ForceMode2D.Impulse);
    yield return new WaitForSeconds(0.2f);
    _knockbackCoroutine = null;
}
```
