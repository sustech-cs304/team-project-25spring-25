## 1. Metrics

- **Lines of Code**: 3,260 lines (calculated from all C# scripts in `Assets/Scripts`)
- **Number of Source Files**: 45 `.cs` files
- **Cyclomatic Complexity**: Average 3.4, Max 9 (measured using Visual Studio Code Metrics Tool)
- **Number of Dependencies**: 61(listed in Unity `Packages/manifest.json`, including `com.unity.inputsystem`, `cinemachine`, etc.)

## 2. Documentation

### a. Documentation for End Users

We provide a `README.md` that contains instructions on how to install and play the game, including controls, features.

📎 [User Guide on GitHub](https://github.com/sustech-cs304/team-project-25spring-25/blob/develop/README.md)

### b. Documentation for Developers

We include a `DEVELOPER_GUIDE.md` that explains the folder structure, module responsibilities (e.g., `AIController.cs`, `RaceManager.cs`) and how to extend game features.

📎 [Developer Guide on GitHub](https://github.com/sustech-cs304/team-project-25spring-25/blob/develop/DEVELOPER_GUIDE.md)

## 3. Tests

We used **Unity Test Framework (UTF)** to implement automated testing.

- **Tools**: Unity Test Framework
- **Test Types**:
  - EditMode Tests: for core logic (e.g., score calculation, race manager state)
  - PlayMode Tests: for runtime behavior (e.g., car movement, AI path-following)

Test coverage includes:
- Race start/stop logic
- AI controller path decision
- Player controller

A test code example like:

```c#
[UnityTest]
public IEnumerator TestAiMovementToTarget()
{
    aiCarObject.transform.position = Vector3.zero;
    aiCarObject.transform.forward = Vector3.forward;
    for (int i = 0; i < 30; i++)
    {
        yield return null;
    }
    Assert.Greater(testCar.rearLeftCollider.rotationSpeed, -1f);
    Assert.LessOrEqual(Mathf.Abs(testCar.steeringAxis), 1f);
}
```

Our tests are partially automated using Unity Test Framework (UTF).

- **Coverage**: We implemented unit tests for race start logic, score calculation, and AI pathing logic.
- **Tools**: UTF + Code Coverage package
- **Coverage Report**: 68% code coverage overall, 92% for critical game manager logic.

We found and fixed 3 logic bugs during testing:
- AI controller incorrectly skipped waypoints.
- Race timer could start multiple times if called in rapid succession.
- Collision handler triggered multiple times for one event.

Hence, our tests are reasonably effective in catching logic issues and maintaining core gameplay integrity.

📎 [Test Source Code on GitHub](https://github.com/sustech-cs304/team-project-25spring-25/tree/develop/Assets/Scripts/Tests)

## 4. Build

### Build Toolchain:
- We use the **Unity Editor's built-in build system** to generate platform-specific builds.
- Build is performed through the Unity Editor's **File > Build Settings** menu.
- Target platforms: Windows, MacOS, and WebGL.

### Build Tasks:
- ✅ Compile game code and assets
- ✅ Build executable using selected platform settings
- ✅ Playmode and EditMode tests run manually before each release

### Final Artifacts:
- Windows build: `SUSTechRacing.exe` and associated data folder
- MacOS build: `.app` bundle
- WebGL build: export folder for browser hosting

> Artifacts are stored under the `/Build/` directory inside the Unity project root.

## 5. Deployment

### Deployment Approach:
- Our game is deployed using **Unity WebGL Build**.
- After building the game for WebGL, we use **Unity WebGL Publisher** to host the game on a public website.
- This enables **cross-platform accessibility** — users can play the game directly in modern web browsers without installing any software.

### Deployment Steps:
1. Select `WebGL` as the target platform in Unity (`File > Build Settings > WebGL`).
2. Click **Build and Run** to generate the WebGL build files (`index.html`, `Build`, `TemplateData` folders).
3. Upload the WebGL build to Unity's official WebGL hosting service via **WebGL Publisher**.
4. Share the generated public URL for users to access the game.

### Final Result:
- The game can be accessed and played from any major operating system (Windows, macOS, Linux) via browser.
- 📎 [Live Game Link (Example)](https://play.unity.com/en/games/d8241a9d-bcc3-4b31-b4b8-4b713f57e10a/sustrace/edit) 

> ⚠️ We chose WebGL deployment over traditional desktop installers to ensure better accessibility and eliminate OS compatibility issues.