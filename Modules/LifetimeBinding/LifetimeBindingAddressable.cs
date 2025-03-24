using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace LFramework.LifetimeBinding
{
    public static class LifetimeBindingAddressable
    {
        /// <summary>
        ///     Binds the lifetime of the handle to the <see cref="gameObject" />.
        /// </summary>
        /// <param name="self">The async operation handle to bind</param>
        /// <param name="gameObject">The GameObject to bind the handle's lifetime to</param>
        /// <param name="isScene">Whether the handle is for a scene</param>
        /// <returns>The original handle for chaining</returns>
        /// <exception cref="ArgumentNullException">Thrown when gameObject is null</exception>
        public static AsyncOperationHandle BindTo(this AsyncOperationHandle self, GameObject gameObject, bool isScene)
        {
            if (gameObject == null)
            {
                ReleaseHandle(self, isScene);

                throw new ArgumentNullException(nameof(gameObject),
                    $"{nameof(gameObject)} is null so the handle can't be bound and will be released immediately.");
            }

            if (!gameObject.TryGetComponent(out LifetimeBinding lifetimeBinding))
                lifetimeBinding = gameObject.AddComponent<LifetimeBinding>();

            return BindTo(self, lifetimeBinding, isScene);
        }

        /// <summary>
        ///     Binds the lifetime of the generic handle to the <see cref="gameObject" />.
        /// </summary>
        /// <param name="self">The typed async operation handle to bind</param>
        /// <param name="gameObject">The GameObject to bind the handle's lifetime to</param>
        /// <typeparam name="T">The type of the async operation result</typeparam>
        /// <returns>The original handle for chaining</returns>
        /// <exception cref="ArgumentNullException">Thrown when gameObject is null</exception>
        public static AsyncOperationHandle<T> BindTo<T>(this AsyncOperationHandle<T> self, GameObject gameObject)
        {
            bool isScene = typeof(T) == typeof(SceneInstance);

            if (gameObject == null)
            {
                ReleaseHandle(self, isScene);

                throw new ArgumentNullException(nameof(gameObject),
                    $"{nameof(gameObject)} is null so the handle can't be bound and will be released immediately.");
            }

            ((AsyncOperationHandle)self).BindTo(gameObject, isScene);

            return self;
        }

        /// <summary>
        ///     Binds the lifetime of the handle to the <see cref="lifetimeBinding" />.
        /// </summary>
        /// <param name="self">The async operation handle to bind</param>
        /// <param name="lifetimeBinding">The lifetime binding component to bind to</param>
        /// <param name="isScene">Whether the handle is for a scene</param>
        /// <returns>The original handle for chaining</returns>
        /// <exception cref="ArgumentNullException">Thrown when lifetimeBinding is null</exception>
        public static AsyncOperationHandle BindTo(this AsyncOperationHandle self, LifetimeBinding lifetimeBinding, bool isScene)
        {
            if (lifetimeBinding == null)
            {
                ReleaseHandle(self, isScene);
                throw new ArgumentNullException(nameof(lifetimeBinding),
                    $"{nameof(lifetimeBinding)} is null so the handle can't be bound and will be released immediately.");
            }

            void OnRelease()
            {
                ReleaseHandle(self, isScene);

                lifetimeBinding.EventRelease -= OnRelease;
            }

            lifetimeBinding.EventRelease += OnRelease;
            return self;
        }

        /// <summary>
        ///     Binds the lifetime of the generic handle to the <see cref="lifetimeBinding" />.
        /// </summary>
        /// <param name="self">The typed async operation handle to bind</param>
        /// <param name="lifetimeBinding">The lifetime binding component to bind to</param>
        /// <typeparam name="T">The type of the async operation result</typeparam>
        /// <returns>The original handle for chaining</returns>
        /// <exception cref="ArgumentNullException">Thrown when lifetimeBinding is null</exception>
        public static AsyncOperationHandle<T> BindTo<T>(this AsyncOperationHandle<T> self, LifetimeBinding lifetimeBinding)
        {
            bool isScene = typeof(T) == typeof(SceneInstance);

            if (lifetimeBinding == null)
            {
                ReleaseHandle(self, isScene);

                throw new ArgumentNullException(nameof(lifetimeBinding),
                    $"{nameof(lifetimeBinding)} is null so the handle can't be bound and will be released immediately.");
            }

            ((AsyncOperationHandle)self).BindTo(lifetimeBinding, isScene);
            return self;
        }

        /// <summary>
        ///     Binds the lifetime of the AssetReference to the <see cref="gameObject" />.
        /// </summary>
        /// <param name="self">The asset reference to bind</param>
        /// <param name="gameObject">The GameObject to bind the asset reference's lifetime to</param>
        /// <returns>The original asset reference for chaining</returns>
        /// <exception cref="ArgumentNullException">Thrown when gameObject is null</exception>
        public static AssetReference BindTo(this AssetReference self, GameObject gameObject)
        {
            if (gameObject == null)
            {
                self.ReleaseAsset();

                throw new ArgumentNullException(nameof(gameObject),
                    $"{nameof(gameObject)} is null so the handle can't be bound and will be released immediately.");
            }

            if (!gameObject.TryGetComponent(out LifetimeBinding lifetimeBinding))
                lifetimeBinding = gameObject.AddComponent<LifetimeBinding>();

            void OnRelease()
            {
                self.ReleaseAsset();

                lifetimeBinding.EventRelease -= OnRelease;
            }

            lifetimeBinding.EventRelease += OnRelease;

            return self;
        }

        /// <summary>
        /// Safely releases a handle based on whether it's a scene or not
        /// </summary>
        private static void ReleaseHandle(AsyncOperationHandle handle, bool isScene)
        {
            if (isScene)
                Addressables.UnloadSceneAsync(handle);
            else
                Addressables.Release(handle);
        }
    }
}