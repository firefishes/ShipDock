using ShipDock;
using UnityEngine;

public class Sample_AssetsLoader : ShipDockAppComponent
{
    public override void EnterGameHandler()
    {
        base.EnterGameHandler();

        //手动使用资源加载器加载 AB 包
        AssetsLoader loader = new AssetsLoader();
        loader.Add("sample_res");
        //侦听加载成功事件
        loader.CompleteEvent.AddListener(OnAssetsLoaderCompleted);
        //启动加载器
        loader.Load(out int statu);

        "error: AssetsLoader load statu is {0}".Log(statu != 0, statu.ToString());
    }

    private void OnAssetsLoaderCompleted(bool success, AssetsLoader loader)
    {
        if (success)
        {
            //销毁加载器
            loader.Reclaim();

            string abName = "sample_res";
            string assetName = "Cube";

            AssetBundles abs = ShipDockApp.Instance.ABs;
            //从资源包管理器模组获取资源母本
            GameObject raw = abs.Get(abName, assetName);
            //通过资源母本创建实例
            GameObject model = Instantiate(raw);

            raw = abs.Get<GameObject>(abName, assetName);
            model = Instantiate(raw);
            model.transform.position = new Vector3(5f, 0f, 0f);

            //通过资源包获取资源引用器并创建资源实例
            model = abs.GetAndQuote<GameObject>(abName, assetName, out AssetQuoteder quoteder);
            model.transform.position = new Vector3(-5f, 0f, 0f);

            //通过已创建的资源引用器创建资源实例
            GameObject modelFromQuoteder = quoteder.Instantiate<GameObject>();
            modelFromQuoteder.transform.position = new Vector3(0f, 5f, 0f);

            "log: 引用器当前实例数 {0}".Log(quoteder.Count.ToString());

            //3秒后销毁使用资源引用器创建的第一个资源实例
            TimeUpdater.New(3f, () => 
            {
                //销毁使用资源引用器创建的资源实例
                model.DestroyFromQuote(true);
                "log: 相关资源已销毁，引用器当前实例数 {0}".Log(quoteder.Count.ToString());
            });

            //5秒后移除资源包并卸载所有已创建的实例
            TimeUpdater.New(5f, () => {

                //卸载资源引用器
                abs.UnloadQuote(abName, assetName);
                "log: 引用器当前实例数 {0}".Log(quoteder.Count.ToString());

                //移除资源包
                abs.Remove(abName);
            });
        }
        else { }
    }
}
