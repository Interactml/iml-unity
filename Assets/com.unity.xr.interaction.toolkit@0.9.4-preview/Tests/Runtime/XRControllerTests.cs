﻿using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
#if LIH_PRESENT
using UnityEngine.Experimental.XR.Interaction;
#endif 
#if LIH_PRESENT_V2API
using UnityEngine.SpatialTracking;
#endif

namespace UnityEngine.XR.Interaction.Toolkit.Tests
{
    [TestFixture]
    public class XRControllerTests
    {

        public class XRControllerWrapper : XRController
        {
            public void FakeUpdate()
            {
                UpdateTrackingInput();
            }
        }

        static Vector3 testpos = new Vector3(1.0f, 2.0f, 3.0f);
        static Quaternion testrot = new Quaternion(0.09853293f, 0.09853293f, 0.09853293f, 0.9853293f);

#if LIH_PRESENT_V1API
        internal class TestPoseProvider : BasePoseProvider
        {          
            public override bool TryGetPoseFromProvider(out Pose output)
            {
                Pose tmp = new Pose();
                tmp.position = testpos;
                tmp.rotation = testrot;
                output = tmp;
                return true;
            }
        }
#elif LIH_PRESENT_V2API
        internal class TestPoseProvider : BasePoseProvider
        {          
            public override PoseDataFlags GetPoseFromProvider(out Pose output)
            {
                Pose tmp = new Pose();
                tmp.position = testpos;
                tmp.rotation = testrot;
                output = tmp;
                return  PoseDataFlags.Position | PoseDataFlags.Rotation;
            }
        }
#endif


        internal static void CreateGOSphereCollider(GameObject go, bool isTrigger = true)
        {
            SphereCollider collider = go.AddComponent<SphereCollider>();
            collider.radius = 1.0f;
            collider.isTrigger = isTrigger;
        }

        internal static XRDirectInteractor CreateDirectInteractorWithWrappedXRController()
        {
            GameObject interactorGO = new GameObject();
            CreateGOSphereCollider(interactorGO);
            XRControllerWrapper controllerWrapper = interactorGO.AddComponent<XRControllerWrapper>();
            XRDirectInteractor interactor = interactorGO.AddComponent<XRDirectInteractor>();
#if LIH_PRESENT
            TestPoseProvider tpp = interactorGO.AddComponent<TestPoseProvider>();
            controllerWrapper.poseProvider = tpp;
#endif
            return interactor;
        }



        [TearDown]
        public void TearDown()
        {
            TestUtilities.DestroyAllInteractionObjects();
        }


        [UnityTest]
        public IEnumerator XRControllerPoseProviderTest()
        {
            TestUtilities.CreateInteractionManager();            
            var directInteractor = CreateDirectInteractorWithWrappedXRController();
#if LIH_PRESENT
            var controllerWrapper = directInteractor.GetComponent<XRControllerWrapper>();
            if(controllerWrapper)
            {
                var tpp = directInteractor.GetComponent<TestPoseProvider>();
                Assert.That(controllerWrapper.poseProvider, Is.EqualTo(tpp));

                controllerWrapper.FakeUpdate();
                                
                Assert.That(controllerWrapper.gameObject.transform.position, Is.EqualTo(testpos));
                Assert.That(controllerWrapper.gameObject.transform.rotation.Equals(testrot));
            }
#endif

            yield return new WaitForSeconds(0.1f);
        }
    }
}
