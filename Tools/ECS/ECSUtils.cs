#if ECS
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GothmogToolkit.Tools.Helpers.ECS
{
    public static class EcsUtils
    {
        public static EntityManager Manager => World.DefaultGameObjectInjectionWorld.EntityManager;

        public static bool IsNull(this Entity entity) => entity == Entity.Null;
        public static Entity CreateEntity() => Manager.CreateEntity();
    
        public static Entity CreateEntity<T>(T data) where T : unmanaged, IComponentData
        {
            var e = CreateEntity();
            e.AddComponent(data);
            return e;
        }
    
        public static Entity CreateEntity<T>(T data, EntityCommandBuffer ecb) where T : unmanaged, IComponentData
        {
            var e =ecb.CreateEntity();
            e.AddComponent(data, ecb);
            return e;
        }
        public static void DestroyEntity(this Entity e) => Manager.DestroyEntity(e);
        public static void DestroyEntity(this Entity e, EntityCommandBuffer commandBuffer) => commandBuffer.DestroyEntity(e);

        public static T GetAspect<T>(this Entity entity)where T: unmanaged, IAspect, IAspectCreate<T> 
            => Manager.GetAspect<T>(entity);
        public static bool HasComponent<T>(this Entity entity) where T : unmanaged, IComponentData 
            => Manager.HasComponent<T>(entity); 
        public static bool HasComponent<T>(this Entity entity, EntityManager manager) where T : unmanaged, IComponentData 
            => manager.HasComponent<T>(entity); 
        public static T GetComponent<T>(this Entity entity) where T : unmanaged, IComponentData => Manager.GetComponentData<T>(entity);

        public static bool TryGetComponent<T>(this Entity entity, EntityManager manager, out T component) where T : unmanaged, IComponentData
        {
            if (manager.HasComponent<T>(entity))
            {
                component = Manager.GetComponentData<T>(entity);
                return true;
            }

            component = default;
            return false;
        }
        
        public static bool TryGetComponent<T>(this Entity entity, out T component) where T : unmanaged, IComponentData
        {
            return entity.TryGetComponent<T>(Manager, out component);
        }
        public static bool TryGet<T>(this Entity entity, out T data) where T : unmanaged, IComponentData
            => entity.TryGetComponent(out data);

        public static void AddComponent<T>(this Entity entity) where T : unmanaged, IComponentData 
            => Manager.AddComponent<T>(entity);
        public static void AddComponent<T>(this Entity entity, EntityCommandBuffer commandBuffer) where T : unmanaged, IComponentData 
            => commandBuffer.AddComponent<T>(entity);

        public static bool TryAddComponent<T>(this Entity entity, EntityCommandBuffer commandBuffer)
            where T : unmanaged, IComponentData
        {
            if (entity.HasComponent<T>())
                return true;
            commandBuffer.AddComponent<T>(entity);
            return true;
        } 
        public static bool TryAddComponent<T>(this Entity entity, EntityManager manager)
            where T : unmanaged, IComponentData
        {
            if (entity.HasComponent<T>())
                return true;
            manager.AddComponent<T>(entity);
            return true;
        } 
        public static bool TryAddComponent<T>(this Entity entity)
            where T : unmanaged, IComponentData
        {
            return TryAddComponent<T>(entity, Manager);
        } 
        
        public static void AddComponent<T>(this Entity entity, T data) where T : unmanaged, IComponentData
        {
            entity.AddComponent(data,Manager);
        }
        public static void AddComponent<T>(this Entity entity, T data, EntityManager manager) where T : unmanaged, IComponentData
        {
            manager.AddComponent<T>(entity);
            manager.SetComponentData(entity, data);
        }
        public static void AddComponent<T>(this Entity entity, T data, EntityCommandBuffer commandBuffer) where T : unmanaged, IComponentData
        {
            commandBuffer.AddComponent<T>(entity);
            commandBuffer.SetComponent(entity, data);
        }
    
        public static void ModifyComponent<T>(this Entity entity, RefAction<T> change) where T : unmanaged, IComponentData
        {
            var data = Manager.GetComponentData<T>(entity);
            change(ref data);
            Manager.SetComponentData(entity, data);
        }
        
        public static void ModifyComponent<T>(this Entity entity, RefAction<T> change, EntityManager manager) where T : unmanaged, IComponentData
        {
            var data = manager.GetComponentData<T>(entity);
            change(ref data);
            Manager.SetComponentData(entity, data);
        }
        
        public static void AddOrModifyComponent<T>(this Entity entity, RefAction<T> change, EntityManager manager) where T : unmanaged, IComponentData
        {
            if(!entity.HasComponent<T>(manager))
                entity.AddComponent(new T(), manager);

            entity.ModifyComponent(change, manager);
        }
        
        public static void AddOrModifyComponent<T>(this Entity entity, RefAction<T> change, EntityManager manager, EntityCommandBuffer buffer) where T : unmanaged, IComponentData
        {
            if(!entity.HasComponent<T>(manager))
                entity.AddComponent(new T(), buffer);

            entity.ModifyComponent(change, buffer);
        }

        public static void ModifyComponent<T>(this Entity entity, RefAction<T> change, EntityCommandBuffer commandBuffer) where T : unmanaged, IComponentData
        {
            var data = Manager.GetComponentData<T>(entity);
            change(ref data);
            commandBuffer.SetComponent(entity, data);
        }
    
        public static void RemoveComponent<T>(this Entity entity) where T : unmanaged, IComponentData
            => Manager.RemoveComponent<T>(entity);
        public static void RemoveComponent<T>(this Entity entity, EntityCommandBuffer commandBuffer) where T : unmanaged, IComponentData
            => commandBuffer.RemoveComponent<T>(entity);
    
        public static bool TryRemoveComponent<T>(this Entity entity) where T : unmanaged, IComponentData
        {
            if (!entity.HasComponent<T>()) 
                return false;
            
            entity.RemoveComponent<T>(); 
            return true;
        }
        public static bool TryRemoveComponent<T>(this Entity entity, EntityCommandBuffer commandBuffer) where T : unmanaged, IComponentData
        {
            if (!entity.HasComponent<T>()) 
                return false;
            
            commandBuffer.RemoveComponent<T>(entity); 
            return true;;
        }
    
    
        public static DynamicBuffer<T> AddBuffer<T>(this Entity entity) where T : unmanaged, IBufferElementData
            => Manager.AddBuffer<T>(entity);
        public static DynamicBuffer<T> AddBuffer<T>(this Entity entity, EntityCommandBuffer commandBuffer) where T : unmanaged, IBufferElementData
            => commandBuffer.AddBuffer<T>(entity);
        public static DynamicBuffer<T> GetBuffer<T>(this Entity entity) where T : unmanaged, IBufferElementData
            => Manager.GetBuffer<T>(entity);
        public static DynamicBuffer<T> GetBuffer<T>(this Entity entity, EntityManager manager) where T : unmanaged, IBufferElementData
            => manager.GetBuffer<T>(entity);
        public static bool TryGetBuffer<T>(this Entity entity, out DynamicBuffer<T> buffer)
            where T : unmanaged, IBufferElementData
        {
            if (entity.HasBuffer<T>())
            {
                buffer = Manager.GetBuffer<T>(entity);
                return true;
            }

            buffer = default;
            return false;
        }

        public static bool HasBuffer<T>(this Entity entity) where T : unmanaged, IBufferElementData 
            => Manager.HasBuffer<T>(entity);

        public delegate void RefAction<T>(ref T item) where T : unmanaged, IComponentData;

        public static Vector2Int ToVector2Int(this int2 source) => new Vector2Int(source.x, source.y);
        public static int2 ToInt2(this Vector2Int source) => new int2(source.x, source.y);
    }
}
#endif