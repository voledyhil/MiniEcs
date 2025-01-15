# MiniEcs
Very simple, lightweight Entity Component System platform based on archetype architecture.

A solution based on archetypes does not require iterating over all entities and testing them to find out if they have the right components. Instead, we repeat all archetypes that will be much smaller than entities, then return all entities from archetypes. <b>The engine does not cache entities!</b>

## Overview

### World
The EcsWorld class acts as a manager for creating an entity, selecting a collection of specific archetypes, and then retrieving entities.

### Declare Components
```csharp

public class ComponentA : IEcsComponent 
{ 
}

public class ComponentB : IEcsComponent 
{ 
    [BinaryItem] public int Value;
}

public class ComponentC : IEcsComponent 
{ 
}

public class ComponentD : IEcsComponent 
{
    public int Value;
}
```
### Register Components

```csharp
EcsComponentType<ComponentA>.Register();
EcsComponentType<ComponentB>.Register();
EcsComponentType<ComponentC>.Register();
EcsComponentType<ComponentD>.Register();
```
Create world

```csharp
var world = new EcsWorld();
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
entityAB.AddComponent(new ComponentC());
``` 
Remove component from entity 
```csharp
entityAB.RemoveComponent<ComponentC>();
``` 
### Filter
Query defines the set of component types that an archetype must contain in order for its entities to be included in the view. You can also exclude archetypes that contain specific types of components.

Create Filter
```csharp
// Searched archetypes must have components of type "B" and "D", 
// but component "A" must be missing
var filterBDnA = new EcsFilter().AllOf<ComponentB, ComponentD>().NoneOf<ComponentA>();
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
group.ForEach((IEcsEntity entity, ComponentB compB, ComponentD compD) =>
{
    compB.Value++;
    compD.Value++;
});
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
        _filterBDnA = new EcsFilter().AllOf<ComponentB, ComponentD>().NoneOf<ComponentA>();
    }
    public void Update(float deltaTime, EcsWorld world)
    {
        world.Filter(_filterBDnA).ForEach((IEcsEntity entity, ComponentB compB, ComponentD compD) =>
        {
            compB.Value++;
            compD.Value++;
        });
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
### [Serialization. Deserialization. Div Compression](https://github.com/voledyhil/BinarySerializer)
Serialize world with filter 
```csharp
byte[] data = _world.Serialize(filterBDnA);
```
Deserialize to target world
```csharp
EcsWorld targetWorld = new EcsWorld();
targetWorld.Update(data);
```
Partial serialize world with filter
```csharp
Baseline<uint> baseline = new Baseline<uint>();
data = _world.Serialize(filterBDnA, baseline);
```

### Stress Test

The [test](https://github.com/voledyhil/MiniEcs/blob/master/MiniEcs.Benchmark/ComplexTest.cs) involves creating, filtering, and deleting entities.

```
BenchmarkDotNet=v0.12.1, OS=macOS Mojave 10.14.6 (18G2022) [Darwin 18.7.0]
Intel Core i7-8850H CPU 2.60GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=2.2.107
  [Host]     : .NET Core 2.2.5 (CoreCLR 4.6.27617.05, CoreFX 4.6.27618.01), X64 RyuJIT
  DefaultJob : .NET Core 2.2.5 (CoreCLR 4.6.27617.05, CoreFX 4.6.27618.01), X64 RyuJIT
```
|                Method |         Mean |      Error |     StdDev |       Median |    Gen 0 |   Gen 1 | Gen 2 |  Allocated |
|---------------------- |-------------:|-----------:|-----------:|-------------:|---------:|--------:|------:|-----------:|
| MiniEcsForEachOneComp |     62.76 us |   0.475 us |   0.371 us |     62.69 us |   0.1221 |       - |     - |      944 B |
| MiniEcsForEachTwoComp |     86.57 us |   1.181 us |   1.047 us |     86.69 us |   0.1221 |       - |     - |      944 B |
|     MiniEcsStressTest |  1,664.66 us |  33.405 us |  97.443 us |  1,653.71 us | 146.4844 | 50.7813 |     - |   717256 B |
| EntitasForEachOneComp |  2,150.14 us | 152.466 us | 449.550 us |  2,020.88 us |        - |       - |     - |      272 B |
| EntitasForEachTwoComp |  2,730.73 us | 199.546 us | 588.365 us |  2,637.36 us |        - |       - |     - |      272 B |
|     EntitasStressTest | 15,593.49 us | 309.112 us | 631.433 us | 15,421.15 us |  62.5000 | 31.2500 |     - | 10977674 B |

## References
1. Building an ECS #2: Archetypes and Vectorization (https://medium.com/@ajmmertens/building-an-ecs-2-archetypes-and-vectorization-fe21690805f9)

2. ECS back and forth (Part 2 - Where are my entities?)(https://skypjack.github.io/2019-03-07-ecs-baf-part-2/)

3. World, system groups, update order, and the player loop (https://gametorrahod.com/world-system-groups-update-order-and-the-player-loop/)
