# Developer Guide

Welcome to the Developer Guide for the **SUSTech Car Racing Game**. This document is intended to help new developers quickly understand the structure, modules, and development workflow of the project.

---

## 🚗 Project Overview

This is a third-person racing game built with Unity, where players can freely explore a virtual map of Southern University of Science and Technology (SUSTech) by driving through key landmarks, competing against AI or other players online.

---

## 📁 Project Structure

Below is the key structure of the `Scripts/` directory:

Scripts/

├── Editor/         # Custom Unity editor scripts

├── Entity/         # Core game entities (e.g., player, vehicles)

├── Loader/         # Scene/asset loading logic

├── LoginManager/      # User authentication and login handling

├── Manager/        # Game managers (e.g., RaceManager, UIManager)

├── Tests/         # Automated test scripts

├── TestScripts/      # Test helpers or mock components

├── UI/           # UI-related logic and handlers

├── AiController.cs     # Controls AI racers’ movement and decisions

├── CameraFollow.cs     # Third-person camera follow system

├── Car.cs         # Base class for all cars

├── CarNetworkController.cs # Network sync logic for multiplayer

├── CarSync.cs       # Handles car state synchronization

├── CheckPoint.cs      # Checkpoint logic for lap/race progression

├── PlayerController.cs   # Handles user input and car control

├── RpcController.cs    # Manages remote procedure calls in multiplayer

├── Singleton.cs      # Generic Singleton pattern helper

---

## **🧩 Module Responsibilities**

| **Module/Class**             | **Responsibility**                                       |
| ---------------------------- | -------------------------------------------------------- |
| PlayerController.cs          | Player car input, drifting (Space), and boosting (Shift) |
| AiController.cs              | AI car movement and navigation using Unity NavMesh       |
| RaceManager.cs (in Manager/) | Controls race flow, lap count, game states               |
| CarNetworkController.cs      | Multiplayer car state synchronization                    |
| UI/                          | Displays speedometer, minimap, and nitro bar             |
| CheckPoint.cs                | Triggers for lap progression                             |
| Loader/                      | Async loading for assets and scenes                      |
| CameraFollow.cs              | Keeps camera following the player car smoothly           |
| LoginManager/                | Handles login flow and user data persistence             |

## 🔧 Tools & Dependencies

The game is developed using Unity and leverages various Unity packages:

- `com.unity.render-pipelines.universal`: URP for rendering
- `com.unity.cinemachine`: Advanced camera system
- `com.unity.ai.navigation`: AI pathfinding for enemy racers
- `com.unity.textmeshpro`: UI text rendering
- `com.unity.test-framework`: Automated unit testing
- `net.tnrd.nsubstitute`: Mocking framework for tests
- `com.unity.visualscripting`: Optional node-based scripting

Full dependency list can be found in `Packages/manifest.json`.

---

## 🧪 Testing

- Tests are located under `Scripts/Tests/` and `Scripts/TestScripts/`.
- We use Unity Test Runner for play mode and edit mode tests.
- Mocking is done using `NSubstitute`.

To run tests:
1. Open Unity Editor
2. Go to `Window > General > Test Runner`
3. Run tests from the list

---

## ⚙️ Build Instructions

We use Unity's built-in build pipeline:
- For automated builds, create a build script using `BuildPipeline.BuildPlayer()`.
- Output builds are placed in the `/Build/` directory.

Optional CI integration (e.g., GitHub Actions + Unity Builder) is recommended for production deployment.

---

## 🐳 Deployment (Optional)

You can optionally containerize the game server using Docker. For example:

```dockerfile
FROM unityci/editor:2021.3.11f1-windows-mono-1.0.0
COPY . /app
WORKDIR /app
RUN unity -batchmode -buildWindows64Player Build/Game.exe -quit
```

## **🚧 Extending the Game**

Add a New Car
	1.	Create a prefab in Assets/Prefabs/Cars/.
	2.	Attach the Car script and configure physics.
	3.	Register it in the garage UI and car selector.

Add a New Race Track
	1.	Design the environment in Unity Editor.
	2.	Add CheckPoint components throughout the track.
	3.	Update RaceManager to recognize the new track.

Add UI Elements
	1.	Create UI components under Canvas/.
	2.	Hook up logic in scripts under UI/.
	3.	Bind data using TextMeshPro and UnityEvents.

## **🛠 Dev Environment**
	•	Unity Version: 2022.3+
	•	Code Editor: JetBrains Rider or Visual Studio
	•	Test Framework: Unity Test Framework + NSubstitute

## **📝 Coding Guidelines**

- Use PascalCase for class names and public methods.
- Use camelCase for private variables.
- Place all MonoBehaviours in the Scripts/ folder.
- Keep scripts under 300 lines where possible.

## **👥 Contribution Workflow**

1. Fork the repository
2. Create a feature branch (feature/minimap)
3. Commit your changes with clear messages
4. Open a Pull Request

## **📬 Contact & Support**

If you need help understanding the codebase, please contact the team lead or check the internal wiki (coming soon).
