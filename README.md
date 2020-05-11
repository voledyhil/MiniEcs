# MiniEcs
Very simple, lightweight Entity Component System platform based on archetype architecture.

A solution based on archetypes does not require iterating over all entities and testing them to find out if they have the right components. Instead, we repeat all the archetypes that will be much smaller than the entities, then we return all the entities from the archetypes that are created for the set of components that contains the desired ones.
## Overview

### World
The EcsWorld class acts as a manager for creating an entity, selecting a collection of specific archetypes, and then retrieving entities.

Declare Components
```csharp
public static class ComponentType
{
    public const byte A = 0;
    public const byte B = 1;
    public const byte C = 2;
    public const byte D = 3;

    public const byte TotalComponents = 4;
}

public class ComponentA : IEcsComponent
{
    public byte Index => ComponentType.A;
}

public class ComponentB : IEcsComponent
{
    public byte Index => ComponentType.B;
}

public class ComponentC : IEcsComponent
{
    public byte Index => ComponentType.C;
}

public class ComponentD : IEcsComponent
{
    public byte Index => ComponentType.D;
}
```    
Create world

```csharp
var world = new EcsWorld(ComponentType.TotalComponents);
``` 
### Entity
An entity is a collection of unique components

Create entity

```csharp
// Creates a new entity, with an initial set of components
var entityAB = world.CreateEntity(new ComponentA(), new ComponentB());
``` 
Destroy entity
```csharp
entityAB.Destroy()
``` 
### Component
Add component to entity
```csharp
entityAB[ComponentType.C] = new ComponentC();
``` 
Remove component from entity 
```csharp
entityAB[ComponentType.C] = null;
``` 
### Filter
Query defines the set of component types that an archetype must contain in order for its entities to be included in the view. You can also exclude archetypes that contain specific types of components.

Create Filter
```csharp
// Searched archetypes must have components of type "B" and "D", 
// but component "A" must be missing
var filterBDnA = new EcsFilter().AllOf(ComponentType.B, ComponentType.D).NoneOf(ComponentType.A)
```
### Group
Collection of archetypes matching filter criteria

Create Group 
```csharp
var entityABD = world.CreateEntity(new ComponentA(), new ComponentB(), new ComponentD());
var entityAC = world.CreateEntity(new ComponentA(), new ComponentC());
var entityBD0 = world.CreateEntity(new ComponentB(), new ComponentD());
var entityBD1 = world.CreateEntity(new ComponentD(), new ComponentB());
var entityBC = world.CreateEntity(new ComponentC(), new ComponentB());
var entityAB = world.CreateEntity(new ComponentB(), new ComponentA());
var entityAD = world.CreateEntity(new ComponentA(), new ComponentD());

// group contain only entityBD0 and entityBD1
var group = world.Filter(filterBDnA);
```
Enumerate Entities
```csharp
foreach (var entity in group))
{
    var compB = (ComponentB) entity[ComponentType.B];
    var compD = (ComponentD) entity[ComponentType.D];
}
```
### Systems
The system provides logic that converts component data from its current state to its next state - for example, the system can update the positions of all moving objects at their speed times the time interval since the last update.

Declare System
```csharp
public class SystemBD : IEcsSystem
{
    private EcsFilter _filterBDnA;
    public SystemBD()
    {
        _filterBDnA = new EcsFilter().AllOf(ComponentType.B, ComponentType.D).NoneOf(ComponentType.A)
    }
    public void Update(float deltaTime, EcsWorld world)
    {
        foreach (var entity in world.Filter(_filterBDnA)))
        {
            var compB = (ComponentB) entity[ComponentType.B];
            var compD = (ComponentD) entity[ComponentType.D];
        }
    }
}
```
### System Group
Use System Groups to specify the update order of your systems. You can place a systems in a group using the [UpdateInGroup] attribute on the system’s class declaration. You can then use [UpdateBefore] and [UpdateAfter] attributes to specify the update order within the group.

```csharp
private class SystemGroupA : EcsSystemGroup
{
}

[EcsUpdateInGroup(typeof(SystemGroupA))]
[EcsUpdateBefore(typeof(SystemB))]
[EcsUpdateAfter(typeof(SystemC))]
private class SystemA : IEcsSystem
{
    public void Update(float deltaTime, EcsWorld world)
    {
    }
}

[EcsUpdateInGroup(typeof(SystemGroupA))
private class SystemB : IEcsSystem
{
    public void Update(float deltaTime, EcsWorld world)
    {
    }
}

[EcsUpdateInGroup(typeof(SystemGroupA))]
private class SystemC : IEcsSystem
{
    public void Update(float deltaTime, EcsWorld world)
    {
    }
}
```
### Game Loop

Create game loop
```csharp
var engine = new EcsSystemGroup();

engine.AddSystem(new SystemA());
engine.AddSystem(new SystemB());
engine.AddSystem(new SystemC());

// update order C -> A -> B
engine.Update(0.1f, world);
```

### Stress Test

The [test](https://github.com/voledyhil/MiniEcs/blob/master/MiniEcs.Benchmark/ComplexTest.cs) involves creating, filtering, and deleting thousands of entities.

```
BenchmarkDotNet=v0.12.1, OS=macOS Mojave 10.14.6 (18G2022) [Darwin 18.7.0]
Intel Core i7-8850H CPU 2.60GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=2.2.107
  [Host]     : .NET Core 2.2.5 (CoreCLR 4.6.27617.05, CoreFX 4.6.27618.01), X64 RyuJIT
  DefaultJob : .NET Core 2.2.5 (CoreCLR 4.6.27617.05, CoreFX 4.6.27618.01), X64 RyuJIT
```
|            Method |      Mean |     Error |    StdDev |    Gen 0 |   Gen 1 | Gen 2 | Allocated |
|------------------ |----------:|----------:|----------:|---------:|--------:|------:|----------:|
| EntitasStressTest | 16.045 ms | 0.3156 ms | 0.3508 ms |  62.5000 | 31.2500 |     - | 480.82 KB |
| MiniEcsStressTest |  9.275 ms | 0.0571 ms | 0.0506 ms | 156.2500 | 15.6250 |     - | 764.34 KB |

### TODO

-~~Caching Destroyed Entities~~
- Extending EcsWorld, EcsEntity, and EcsSystem Interfaces
- Transition from AoS (array of structures) to SoA (struct of arrays) PERFORMANCE!
- Binary serialization, deserialization, div compression


## References
1. Building an ECS #2: Archetypes and Vectorization (https://medium.com/@ajmmertens/building-an-ecs-2-archetypes-and-vectorization-fe21690805f9)

2. ECS back and forth (Part 2 - Where are my entities?)(https://skypjack.github.io/2019-03-07-ecs-baf-part-2/)

3. World, system groups, update order, and the player loop (https://gametorrahod.com/world-system-groups-update-order-and-the-player-loop/)
