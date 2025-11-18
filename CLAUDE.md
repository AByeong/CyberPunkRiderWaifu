# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**CyberPunk Rider** is a 3D action RPG developed in Unity 6000.0.48f1, featuring a cyberpunk-themed world with VRM avatar support and stylized combat inspired by musou-style gameplay. The game centers around intense action combat with a female protagonist, multiple skills, boss battles, and equipment upgrade systems.

**Key Technologies:**
- Unity 6000.0.48f1 (Required version)
- Universal Render Pipeline (URP) 17.0.4
- VRM 1.0/0.x for character system
- Unity Input System 1.13.1
- Cinemachine 3.1.3 for camera control
- Feel for haptic feedback
- Addressables for resource management

**Platform:** PC (Windows), DirectX 11+

## Core Architecture

### Game Flow
The game follows a cyclical structure:
```
Opening → Lobby → Loading → Dungeon Intro → Gameplay → Boss Appear → Boss Fight → Clear → Lobby
```

**Key Scenes:**
- `Loading.unity` - Always start here for proper initialization
- `KBJ_Lobby.unity` - Main lobby with inventory, shop, and upgrades
- `KBJ_Procedure.unity` - Main gameplay stage
- `KBJ_DueongunClear.unity` - Dungeon completion and rewards

See `GameFlowDiagram.md` for detailed flow visualization.

### Manager Pattern with Singleton
The project uses a `Singleton<T>` base class for all managers. Managers persist across scenes via `DontDestroyOnLoad`.

**Core Managers:**
- `GameManager` - Central game state, player reference, time control
- `UIManager` - UI state management, cursor control, popup system
- `SoundManager` - Audio playback (BGM/SFX), sound pooling
- `SceneMover` - Scene loading with async operations
- `DeliveryManager` - Mission tracking, kill counting, stage progression
- `EnemyManager` - Enemy spawn management via `MonsterSpawner`
- `CurrencyManager` - In-game currency (gold, etc.)

**Manager Access Pattern:**
```csharp
GameManager.Instance.player
UIManager.Instance.CursorLock(true)
SoundManager.Instance.PlayBGM(...)
```

### State Machine Architecture (RobustFSM)
Enemy AI uses a hierarchical state machine pattern based on `MonoFSM<OwnerType>`.

**Base Classes:**
- `MonoFSM<OwnerType>` - FSM container managing state transitions
- `MonoState` - Base state with OnEnter/OnExit/Update lifecycle
- `MonoHState` - Hierarchical state supporting sub-states

**Enemy Types & State Machines:**
1. **Normal Enemies** (`NormalEnemy` + `NormalStateMachine`):
   - States: `IdleState`, `AttackState`, `HitState`, `DownedState`, `DeadState`
   - Simple patrol and attack behavior

2. **Elite Enemies** (`EliteEnemy` + `ElliteStateMachine`):
   - States: `EliteIdleState`, `EliteAttackState`, `EliteHitState`, `EliteDownState`, `EliteDeadState`
   - Advanced behaviors: `BossMagicState`, `Summoner`, `KingStompSMB`, `TornadoSMB`

3. **Boss** (`Boss_Waifu_AI`):
   - Two phases controlled by `BossPhase1` and `BossPhase2`
   - Phase 2 triggers at 50% HP with advanced attack patterns
   - Skills: `Laser`, `Missile`, `EnergyBall`, `BustShotSMB`, `MissileSwarm`, `LaserFireSMB`
   - Special attacks: `CyberKatanaAttack`, `WaifuSpell`

**State Machine Pattern:**
```csharp
// FSM implementation (EliteEnemy example)
public override void AddStates()
{
    AddState<EliteIdleState>();
    AddState<EliteAttackState>();
    AddState<EliteHitState>();
    AddState<EliteDownState>();
    AddState<EliteDeadState>();
    SetInitialState<EliteIdleState>();
}

// State transitions
Machine.ChangeState<EliteAttackState>();
```

### Player System
**Namespace:** `JY`

**Core Components:**
- `PlayerController` - Character movement and state management
- `PlayerInput` - Input handling with blocking system for cutscenes
- `PlayerAttackManager` - Attack execution, combos, skill management
- `PlayerHit` - Damage reception and hit reactions
- `StatComponent` - Player stats (HP, attack power, critical, etc.)

**Input Mapping:**
- Movement: WASD / Arrow Keys
- Attack: Left Mouse Button
- Skills: 1, 2, 3, 4 keys
- Ultimate: R key
- Dodge: Left Shift
- Jump: Space

**Attack System:**
- Basic attacks with combo chains
- 4 skill slots with cooldowns
- Ultimate ability
- Critical hit system
- Weapon-specific modifiers from `EquipmentDataSO`

### Inventory & Equipment
**ScriptableObject-Based Data:**
- `ItemBaseDataSO` - Base item properties
- `EquipmentDataSO` - Weapon/armor stats and bonuses
- Equipment types: Weapon, Armor, Accessories

**Inventory Manager:**
- `InventoryManager` - Handles item storage, equipment changes
- `UI_InventoryPopup` - Visual inventory interface
- `UI_EquipmentSlot` - Equipment UI elements
- `UI_SkillChipSlot` - Skill chip system for ability customization

**Item System:**
- `Item` class for item instances
- `ItemDropManager` - Handles enemy drops
- `ItemCreateManager` - Item instantiation
- `ConsumableItemManager` - Potion usage and effects

### Enemy Data System
Enemies are configured via `EnemyDataSO` ScriptableObjects containing:
- Health points
- Attack power and ranges
- Drop rates (Gold, ETC items, equipment)
- Stagger time and AI behavior parameters
- Animation references

**Enemy Base Class (`Enemy.cs`):**
- Implements `IDamageable` interface
- NavMesh-based pathfinding via `NavMeshAgent`
- Hit flash visual feedback system with material property manipulation
- Damage popup system (world-space canvas)
- Object pooling support via `ObjectPool` reference

**Damage System:**
```csharp
public class Damage
{
    public int DamageValue;
    public EDamageType DamageType;  // Normal, Airborne, Down
    public EDamageCriType DamageCriType;  // Normal, Critical
    public float DamageForce;
    public float AirRiseAmount;
    public GameObject From;
}
```

### UI Architecture
**Popup System:**
- `PopupManager` - Manages popup lifecycle and stacking
- `Popup` base class - All popups inherit from this
- `UIPopupAnimator` - Handles popup animations

**Major UI Components:**
- `StageMainUI` - In-game HUD (HP, skills, minimap)
- `UI_ShopPopup` - Item purchase interface
- `UI_InventoryPopup` - Equipment and item management
- `UI_SkillInspector` - Skill details and upgrades
- `QuestBar` - Mission objectives display
- `DeliveryPopup` - Stage briefing UI

**MiniMap System:**
- `MiniMapCamera` - Dedicated render texture camera
- Real-time top-down view of player and enemies

### Camera & Cutscenes
**Cinemachine Integration:**
- `CinemachineManager` - Virtual camera coordination
- `CinemachineTrigger` - Event-based camera switches
- `CutSceneManager` - Cinematic sequence control
- `BossAppearEnd` - Boss introduction cutscene handling

**Timeline-based Cutscenes:**
Multiple Timeline-controlled scenes for:
- Dungeon intro (`KBJ_DungeonIntro`)
- Boss appearances (`KBJ_Boss1Appear`, `KBJ_Boss2Appear`)
- Victory sequence (`KBJ_DueongunClear`)

### Spawning & Object Pooling
**Enemy Spawning:**
- `MonsterSpawner` - Spawns enemies at designated points
- `FormationManager` - Enemy formation patterns
- `AIManager` - Coordinates AI behavior across multiple enemies
- `ObjectPool` - Reuses enemy instances for performance

**Stage System:**
- `DeliveryMissionDataSO` - Stage objectives (kill counts, etc.)
- `DeliverystageDataSO` - Stage configuration
- `DeliveryRewardSO` - Completion rewards
- `StageBaker` - Pre-processes stage data
- `KillTracker` - Tracks enemy eliminations

## Development Workflow

### Opening the Project
1. Install Unity Hub
2. Install **Unity 6000.0.48f1 exactly** (version-specific project)
3. Open project via Unity Hub
4. Wait for package imports to complete (VRM, URP, Cinemachine, etc.)
5. Open `Assets/01.Scenes/Loading.unity` as the starting scene

### Running the Game
**Always start from `Loading.unity`** to ensure proper manager initialization and scene flow.

### Testing Individual Features
- **Player Combat:** Load `KBJ_Lobby.unity`, enter dungeon portal
- **Boss Fight:** Load `KBJ_Procedure.unity`, progress to boss trigger
- **UI Systems:** Open specific scene, test via Play mode

### Scripting Conventions
**Namespace Usage:**
- Player-related scripts use namespace `JY`
- Enemy state machines use namespace `RobustFSM.Base` and `RobustFSM.Interfaces`
- Most other scripts are in the global namespace

**Coding Patterns:**
- Fields: `_privateField` (underscore prefix for private)
- Properties: `PublicProperty` (PascalCase)
- Protected fields in base classes may use `_protectedField`
- Unity serialized fields: `[SerializeField] private Type _field`

**ScriptableObject Naming:**
- Data classes end with `SO` or `DataSO`
- Example: `EnemyDataSO`, `ItemBaseDataSO`, `EquipmentDataSO`

### Asset Organization
```
Assets/
├── 01.Scenes/           - All game scenes
├── 02.Scripts/          - All C# scripts
│   ├── Core/            - Singleton, SceneMover, SoundManager
│   ├── Player/          - PlayerController, PlayerInput, etc.
│   ├── Enemies/         - Enemy classes and AI
│   │   ├── StateMachine/    - FSM implementation
│   │   └── Data/            - EnemyDataSO
│   ├── UI/              - All UI scripts
│   ├── Manager/         - GameManager, CSVManager, etc.
│   ├── Inventory/       - Item and equipment systems
│   ├── Delivery/        - Mission and stage progression
│   └── CameraCinemachine/ - Camera controllers
├── 03.Prefabs/          - Prefabs (characters, enemies, effects)
│   ├── AnimationModels/ - Character models with animations
│   ├── Models/          - Environment and props
│   └── UI/              - UI prefabs
├── VRM/                 - VRM avatar system
├── Feel/                - Feel haptic feedback assets
├── CyberPunkGirl&Vehicle/ - Main character assets
└── Dark City 2/         - Cyberpunk environment assets
```

### Data Files & CSV Loading
The project uses CSV-based stat loading:
- `CSVManager` handles CSV parsing
- `StatLoader.LoadFromCSVAsync("EnemyStat.csv")` - Enemy stats
- Stats can be decorated with modifiers via `StatModifierDecorator`

### Build Configuration
**Scripting Define Symbols:**
- `DOTWEEN` - DOTween animation library
- `UNITY_POST_PROCESSING_STACK_V2` - Post-processing
- `MOREMOUNTAINS_NICEVIBRATIONS_INSTALLED` - Haptic feedback (Standalone/Android)

**Graphics:**
- Android: Vulkan + OpenGLES3
- Standalone: DirectX 11+

## Important Notes for AI Assistants

### Unity-Specific Considerations
- **Unity version is CRITICAL:** This project requires Unity 6000.0.48f1. Do not suggest upgrading or using different versions.
- **URP Pipeline:** All rendering uses URP. Standard shader references won't work.
- **NavMesh Required:** Enemy AI depends on baked NavMesh. Ensure NavMesh exists in scenes.

### State Machine Implementation
- States are MonoBehaviours, not POCOs
- State lifecycle: `Initialize()` → `OnEnter()` → `Update()` → `OnExit()`
- Always call `AddStates()` and `SetInitialState<>()` in FSM setup
- State transitions via `Machine.ChangeState<StateType>()`

### Singleton Pattern Usage
- Managers use `Singleton<T>` base class
- Access via `ManagerName.Instance`
- Avoid circular dependencies between managers
- `DontDestroyOnLoad` is controlled by `_dontDestroy` bool in Singleton

### VRM Avatar Integration
- VRM models are loaded via UniVRM package
- Character rigs use VRM-compatible bone structure
- Custom materials may need VRM shader compatibility

### Performance Considerations
- Object pooling is used for enemies and projectiles
- Materials use `MaterialPropertyBlock` for hit effects to avoid material instances
- Addressables system for asset loading (though direct references are also used)

### Korean Language Support
- Many comments and debug messages are in Korean
- UI text may require Korean font support
- Some variable names contain Korean (legacy code)

### Cutscene Development
- Use Unity Timeline for cutscenes
- Cinemachine for camera control during cutscenes
- Cutscenes block player input via `PlayerInput.playerControllerInputBlocked`

### Animation System
- Mecanim Animator Controllers for all characters
- `StateMachineBehaviour` used for attack timing (`OnAttackBehaviour`, animation events)
- Boss uses layered animation controllers for phase transitions

### Debugging Enemy AI
- Editor Gizmos show current FSM state above enemy (see `MonoFSM.OnDrawGizmos`)
- Use Unity's NavMesh debugging visualizer
- Check `EnemyDataSO` for AI parameter tuning

### Common Pitfalls
1. **Not starting from Loading.unity** - Managers may not initialize properly
2. **Missing NavMesh** - Enemies will fail to pathfind
3. **ScriptableObject null references** - Assign all SO fields in Inspector
4. **State machine not initialized** - Ensure `AddStates()` called before use
5. **Material property names** - Verify shader property names match (e.g., `_Color`, `_BaseColor`)

## References
- **Game Flow Diagram:** See `GameFlowDiagram.md` for complete scene flow
- **README:** See `README.md` for project overview and setup
- **Unity Documentation:** URP, Input System, Cinemachine documentation applies
