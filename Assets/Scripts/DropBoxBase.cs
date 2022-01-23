using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;

public class DropBoxBase : MonoBehaviour {

    [SerializeField]
    protected int itemValue;

    public virtual void SetUpDropBox<T>(T[] t) where T : class {

        //this.OnTriggerEnter2DAsObservable()
        //    .Subscribe(col => {
        //        if (col.TryGetComponent(out PlayerController_All playerController)) {
        //            TriggerDropBoxEffect(playerController);
        //        }
        //    }).AddTo(this);

        //this.OnDestroyAsObservable()
        //    .Subscribe(_ => Debug.Log("Žæ“¾ ”j‰ó‚³‚ê‚Ü‚µ‚½"));
    }


    protected virtual void TriggerDropBoxEffect(PlayerController_All playerController) {

        Destroy(gameObject);
    }

    public virtual void TriggerDropBoxEffect() {

        Destroy(gameObject);
    }

    public virtual void TriggerDropBoxEffect(StageManager_Presenter presenter) {

        Destroy(gameObject);
    }

    //public virtual void SetUpDropBox() {

    //    this.OnTriggerEnter2DAsObservable()
    //        .Subscribe(col => {
    //            if (col.TryGetComponent(out PlayerController_All playerController)) {
    //                TriggerDropBoxEffect(playerController);
    //            }
    //            if (col.TryGetComponent(out PlayerController player)) {
    //                TriggerDropBoxEffect(player);
    //            }
    //        }).AddTo(this);

    //    this.OnDestroyAsObservable()
    //        .Subscribe(_ => Debug.Log("Žæ“¾ ”j‰ó‚³‚ê‚Ü‚µ‚½")); ;
    //}

    //public virtual void SetUpDropBox<T>(T t) {

    //    this.OnTriggerEnter2DAsObservable()
    //        .Subscribe(col => {
    //            if (col.TryGetComponent(out PlayerController_All playerController)) {
    //                TriggerDropBoxEffect(playerController);
    //            }
    //        }).AddTo(this);

    //    this.OnDestroyAsObservable()
    //        .Subscribe(_ => Debug.Log("Žæ“¾ ”j‰ó‚³‚ê‚Ü‚µ‚½")); ;
    //}
}
