# PlayerMovements

Reusable first-person character controller for Unity 6. Drop onto a GameObject and wire up references in the Inspector.

---

## Setup

1. Add a **CharacterController** and **PlayerMovements** component to your player GameObject.
2. Add a **Camera** as a child of the player (`PlayerCamera` in the example below). Assign it to `Camera Transform`.

   ![Hierarchy](Git_img/Hierarchy.png)

3. Create or reuse an **Input Actions asset** with a `Player` action map. Assign each action reference:
   - `Move` — Vector2
   - `Look` — Vector2
   - `Sprint` — Button (hold)
   - `Crouch` — Button (toggle)
   - `Jump` — Button
4. **Set all speed fields** in the Inspector — they default to `0` and nothing will work otherwise.

   ![Inspector](Git_img/Inspector.png)

## How it works

**Movement** — `CharacterController.Move` is called every frame with a direction vector relative to the camera, scaled by the active speed (`walkingSpeed` / `runningSpeed` / `crouchingSpeed`).

**Camera** — Horizontal look rotates the player body (`transform.Rotate`). Vertical look tilts only the camera, clamped to ±90°.

**Jump & Gravity** — Vertical velocity accumulates gravity each frame. Jumping sets `_verticalVelocity = jumpForce` when grounded. Ceiling hits reset it to `initialFallVelocity` to prevent sticking.

**Crouch** — Toggle-based. Changes `CharacterController.height` and `center` via `Mathf.Lerp` each frame. Standing up is blocked if a `Physics.CapsuleCast` detects overhead collision.

**Input** — Uses `InputActionReference` callbacks wired in `OnEnable`/`OnDisable`. Input values are stored each frame and consumed in `Update`.

**Debug Gizmos** — Scene view shows the capsule cast used for crouch/stand detection. Green = can stand up, red = blocked.

<table><tr>
<td><img src="Git_img/CastsphereCanStandUp.png" alt="Can stand up"/><br><sub>Green — overhead is clear</sub></td>
<td><img src="Git_img/CastsphereCanNotStandUp.png" alt="Cannot stand up"/><br><sub>Red — overhead blocked, crouch locked</sub></td>
</tr></table>

---

## Configuration reference

### Speed

| Field | Description |
|---|---|
| `walkingSpeed` | Base movement speed |
| `runningSpeed` | Speed while holding Sprint |
| `crouchingSpeed` | Speed while crouching |
| `mouseSensitivity` | Mouse look sensitivity |

### Jump and Fall

| Field | Default | Description |
|---|---|---|
| `jumpForce` | `5` | Initial upward velocity on jump |
| `gravity` | `-12` | Gravity acceleration (units/s²) |
| `initialFallVelocity` | `-2` | Vertical velocity set on landing / ceiling hit |

### Crouch

| Field | Default | Description |
|---|---|---|
| `standingHeight` | `2` | CharacterController height when standing |
| `crouchingHeight` | `1` | CharacterController height when crouching |
| `crouchTransitionSpeed` | `10` | Lerp speed for height transition |
| `cameraOffset` | `0.2` | Distance below top of capsule for camera position |

### References

| Field | Description |
|---|---|
| `cameraTransform` | Child camera Transform |
| `moveAction` | InputActionReference — Vector2 |
| `lookAction` | InputActionReference — Vector2 |
| `sprintAction` | InputActionReference — Button |
| `crouchAction` | InputActionReference — Button |
| `jumpAction` | InputActionReference — Button |
