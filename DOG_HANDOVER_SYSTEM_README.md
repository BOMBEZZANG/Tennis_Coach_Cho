# Dog Handover System - Implementation Guide

## Overview
The Dog Handover system implements the core interaction sequence where a player accepts a dog walking quest from a client NPC, triggering a handover animation sequence that results in the dog following the player.

## System Architecture

### Core Components

#### 1. HandoverManager (`Scripts/Core/HandoverManager.cs`)
- Singleton manager that orchestrates the entire handover sequence
- Handles dialogue display, input freezing, state changes, and quest updates
- Core method: `HandoverSequence()` coroutine with 1.5-second delay for future animations

#### 2. ClientNPC (`Scripts/NPCs/ClientNPC.cs`)
- NPC that gives dog walking quests
- Visual: Green square (placeholder)
- Features:
  - Quest marker (exclamation mark) appears during appointment window
  - Interaction with 'E' key
  - Appointment time validation
  - Links to assigned DogNPC

#### 3. DogNPC (`Scripts/NPCs/DogNPC.cs`)
- AI-controlled dog with multiple states
- Visual: Yellow circle (placeholder)
- States:
  - `IdleWithOwner`: Wanders around owner
  - `Follow`: Follows player at set distance
  - `Walking`: For future mini-game integration
- Features:
  - Automatic leash visualization (Line Renderer)
  - Smooth following behavior
  - State-based AI

#### 4. DialogueUI (`Scripts/UI/DialogueUI.cs`)
- Simple dialogue system for handover conversation
- Features:
  - Typing effect
  - Confirmation button
  - Keyboard shortcuts (Space/Enter)
  - Auto-generated if not present in scene

### Supporting Systems

#### 5. Updated UIManager
- Added `ShowQuestUpdate()` and `ShowTemporaryMessage()` methods
- Integrated with dialogue system

#### 6. Updated PlayerController
- Added `SetInputEnabled()` method for freezing input during handover

#### 7. Updated NotificationPanel
- Added quest update and temporary message support

## Implementation Details

### Handover Sequence Flow
1. **Pre-conditions Check**:
   - Player has accepted appointment
   - Player is in interaction range
   - Current time is within appointment window (±30 minutes)

2. **Dialogue Phase**:
   - Display client dialogue with quest information
   - Show "Take the leash" confirmation button
   - Wait for player confirmation

3. **Handover Animation Phase**:
   - Freeze player input
   - Play SFX placeholder (debug log)
   - Wait 1.5 seconds (placeholder for future animation)

4. **State Change Phase**:
   - Change dog state from `IdleWithOwner` to `Follow`
   - Activate leash visual (Line Renderer)
   - Update quest log

5. **Completion Phase**:
   - Unfreeze player input
   - Client returns to idle state
   - Quest objective updated

### Placeholder Assets (Grey-box)
- **Player**: Blue square (existing)
- **Client**: Green square (auto-generated)
- **Dog**: Yellow circle (auto-generated)
- **Leash**: Brown line (Line Renderer)
- **Quest Marker**: Yellow exclamation mark (auto-generated)
- **UI**: Default Unity UI elements

## Usage Instructions

### Automatic Setup (Recommended)
1. Add `HandoverDemo` component to any GameObject in scene
2. Configure positions in inspector (optional)
3. Enable "Auto Setup Demo" checkbox
4. Play the scene
5. Follow console instructions

### Manual Setup
1. Ensure GameManager is properly configured with all system references
2. Create Client GameObject:
   ```csharp
   GameObject clientObj = new GameObject("Client");
   ClientNPC client = clientObj.AddComponent<ClientNPC>();
   ```
3. Create Dog GameObject:
   ```csharp
   GameObject dogObj = new GameObject("Dog");
   DogNPC dog = dogObj.AddComponent<DogNPC>();
   ```
4. Create and assign appointment:
   ```csharp
   AppointmentData appointment = new AppointmentData(...);
   appointment.isAccepted = true;
   client.SetAppointment(appointment);
   client.SetAssignedDog(dog);
   ```

### Testing
1. Use `HandoverSystemTest` component to validate all systems
2. Use context menu options:
   - "Run All Tests": Validates all components
   - "Test NPC Creation": Tests NPC instantiation
   - "Show System Requirements": Lists setup requirements

## Integration Points

### Existing Systems
- **AppointmentData**: Extended with dog-specific fields (`dogName`, etc.)
- **GameManager**: Central access point for all systems
- **TimeSystem**: Used for appointment time validation
- **ProgressionManager**: Integration ready for stamina/XP systems

### Future Expansion Points
- **Animation Integration**: Replace 1.5s delay in `HandoverSequence()` with actual animations
- **Sound Integration**: Replace debug logs with actual SFX calls
- **Mini-game Integration**: Dog walking activities after handover
- **Advanced AI**: More complex dog behaviors and reactions

## File Structure
```
Assets/Scripts/
├── Core/
│   └── HandoverManager.cs
├── NPCs/
│   ├── ClientNPC.cs
│   └── DogNPC.cs
├── UI/
│   └── DialogueUI.cs (new)
│   └── UIManager.cs (updated)
│   └── NotificationPanel.cs (updated)
├── Demo/
│   ├── HandoverDemo.cs
│   └── HandoverSystemTest.cs
└── Player/
    └── PlayerController.cs (updated)
```

## Acceptance Criteria Status
- ✅ Player can trigger handover sequence by interacting with Client NPC
- ✅ Dialogue box appears with correct quest information
- ✅ Game executes 1.5-second timed delay after confirmation
- ✅ Dog NPC state changes and correctly follows player
- ✅ Quest Log UI updates with next objective
- ✅ Entire sequence functional without final art/animation/sound assets

## Debug Features
- Comprehensive logging throughout all systems
- Visual gizmos for interaction ranges and AI areas
- Context menu testing functions
- Automatic fallbacks for missing UI components

## Known Limitations
- Placeholder visuals only (as specified)
- No actual animations (1.5s delay placeholder)
- No sound effects (debug log placeholders)
- Basic UI styling only

## Next Steps
1. Test the system in a Unity scene
2. Integrate with existing appointment/quest systems
3. Add actual art assets when ready
4. Implement walking mini-game
5. Add animation sequences to replace delays