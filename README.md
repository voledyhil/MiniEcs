# MiniEcs
Very simple, lightweight Entity Component System platform based on archetype architecture.

A solution based on archetypes does not require iterating over all entities and testing them to find out if they have the right components. Instead, we repeat all archetypes that will be much smaller than entities, then return all entities from archetypes. <b>The engine does not cache entities!</b>

## Overview

### World
The EcsWorld class acts as a manager for creating an entity, selecting a collection of specific archetypes, and then retrieving entities.

### Declare Components
```csharp

public class ComponentA : IEcsComponent { }

public class ComponentB : IEcsComponent { }

public class ComponentC : IEcsComponent { }

public class ComponentD : IEcsComponent { }
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
foreach (var entity in group))
{
    var compB = entity.GetComponent<ComponentB>();
    var compD = entity.GetComponent<ComponentD>();
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
        _filterBDnA = new EcsFilter().AllOf<ComponentB, ComponentD>().NoneOf<ComponentA>();
    }
    public void Update(float deltaTime, EcsWorld world)
    {
        foreach (var entity in world.Filter(_filterBDnA)))
        {
            var compB = entity.GetComponent<ComponentB>();
            var compD = entity.GetComponent<ComponentD>();
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
## Examples
### [MiniEcsPhysics](https://github.com/voledyhil/MiniEcsPhysics)
Implementing a simple 2D isometric physical using MiniEcs framework
![Preview](/images/preview.gif)

### Stress Test

The [test](https://github.com/voledyhil/MiniEcs/blob/master/MiniEcs.Benchmark/ComplexTest.cs) involves creating, filtering, and deleting entities.

```
BenchmarkDotNet=v0.12.1, OS=macOS Mojave 10.14.6 (18G2022) [Darwin 18.7.0]
Intel Core i7-8850H CPU 2.60GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=2.2.107
  [Host]     : .NET Core 2.2.5 (CoreCLR 4.6.27617.05, CoreFX 4.6.27618.01), X64 RyuJIT
  DefaultJob : .NET Core 2.2.5 (CoreCLR 4.6.27617.05, CoreFX 4.6.27618.01), X64 RyuJIT
```
|            Method |     Mean |   Error |   StdDev |   Gen 0 |  Gen 1 | Gen 2 | Allocated |
|------------------ |---------:|--------:|---------:|--------:|-------:|------:|----------:|
| EntitasStressTest | 494.9 us | 9.70 us | 10.79 us |  7.8125 | 2.9297 |     - |  51.64 KB |
| MiniEcsStressTest | 157.7 us | 0.94 us |  1.04 us | 11.2305 | 0.4883 |     - |  52.27 KB |

## References
1. Building an ECS #2: Archetypes and Vectorization (https://medium.com/@ajmmertens/building-an-ecs-2-archetypes-and-vectorization-fe21690805f9)

2. ECS back and forth (Part 2 - Where are my entities?)(https://skypjack.github.io/2019-03-07-ecs-baf-part-2/)

3. World, system groups, update order, and the player loop (https://gametorrahod.com/world-system-groups-update-order-and-the-player-loop/)
