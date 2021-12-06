#define _LOG_MODULARS

using ShipDock.Notices;
using ShipDock.Tools;
using System;
using CreatorHandler = System.Func<int, ShipDock.Notices.INoticeBase<int>>;
using DecoratorHandler = System.Action<int, ShipDock.Notices.INoticeBase<int>>;
using NoticeListener = System.Action<ShipDock.Notices.INoticeBase<int>>;

namespace ShipDock.Modulars
{
    /// <summary>
    /// 
    /// 装饰化模块管理器
    /// 
    /// 模拟 IoC 流程设计的模块管理器。每个模块可定义消息创建器函数和消息装饰函数，并可监听其他广播形式发送的消息
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class DecorativeModulars : IAppModulars
    {
        /// <summary>所有模块</summary>
        private KeyValueList<int, IModular> mModulars;
        private ModularHandlers<CreatorHandler, ModularNoticeCreater> mCreators;
        private ModularHandlers<DecoratorHandler, ModularNoticeDecorater> mDecorators;
        private ModularHandlers<NoticeListener, ModularNoticeListener> mLIsteners;

        public DecorativeModulars()
        {
            mModulars = new KeyValueList<int, IModular>();

            mCreators = new ModularHandlers<CreatorHandler, ModularNoticeCreater>(false, OnCreatorCache);
            mDecorators = new ModularHandlers<DecoratorHandler, ModularNoticeDecorater>(false, OnDecoratorCache);
            mLIsteners = new ModularHandlers<NoticeListener, ModularNoticeListener>(true, OnListenerCache)
            {
                OnAfterHandlerReset = OnAfterListenerReset,
                OnGetHandlerForPriority = OnGetListenerForPriority,
            };
        }

        private NoticeListener OnGetListenerForPriority(ModularNoticeListener target)
        {
            return target.Handler;
        }

        private void OnAfterListenerReset(int noticeName, NoticeListener handler)
        {
            noticeName.Remove(handler);
        }

        private NoticeListener OnListenerCache(NoticeListener a, ModularNoticeListener b)
        {
            a += b.Handler;
            return a;
        }

        private DecoratorHandler OnDecoratorCache(DecoratorHandler a, ModularNoticeDecorater b)
        {
            a += b.Handler;
            return a;
        }

        private CreatorHandler OnCreatorCache(CreatorHandler a, ModularNoticeCreater b)
        {
            a += b.Handler;
            return a;
        }

        public void Dispose()
        {
            mModulars?.Dispose();
            //mNoticeCreaters?.Dispose();
            //mNoticeDecorators?.Dispose();
        }

        /// <summary>
        /// 添加消息装饰器函数
        /// </summary>
        /// <param name="noticeName">消息名</param>
        /// <param name="method">装饰器函数</param>
        public void AddNoticeDecorator(ModularNoticeDecorater target)
        {
            mDecorators.AddHandler(target.NoticeName, target);
        }

        /// <summary>
        /// 移除消息装饰器函数
        /// </summary>
        /// <param name="noticeName"></param>
        /// <param name="method"></param>
        public void RemoveNoticeDecorator(int noticeName, Action<int, INoticeBase<int>> method)
        {
            mDecorators.RemoveHandler(noticeName);
        }

        /// <summary>
        /// 添加消息生成器函数
        /// </summary>
        /// <param name="noticeName"></param>
        /// <param name="method"></param>
        public void AddNoticeCreater(ModularNoticeCreater target)
        {
            mCreators.AddHandler(target.NoticeName, target);
        }

        /// <summary>
        /// 移除消息生成器函数
        /// </summary>
        /// <param name="noticeName"></param>
        /// <param name="method"></param>
        public void RemoveNoticeCreater(int noticeName, Func<int, INoticeBase<int>> method)
        {
            mCreators.RemoveHandler(noticeName);
        }

        public void AddNoticeListener(ModularNoticeListener target)
        {
            mLIsteners.AddHandler(target.NoticeName, target);
        }

        /// <summary>
        /// 添加模块
        /// </summary>
        /// <param name="modulars"></param>
        public void AddModular(params IModular[] modulars)
        {
            IModular modular;
            int max = modulars.Length;
            for (int i = 0; i < max; i++)
            {
                modular = modulars[i];
                if (mModulars.ContainsKey(modular.ModularName))
                {
#if LOG_MODULARS
                    "error".Log(modular.ModularName.ToString().Append(" modular is existed"));
#endif
                    modular.Dispose();
                }
                else
                {
#if LOG_MODULARS
                    "Modular {0} is create".Log(modular.ModularName.ToString());
#endif
                    mModulars[modular.ModularName] = modular;
                    modular.SetModularManager(this);
                    modular.InitModular();
                }
            }
        }

        private void BeforeNotifyModular(int noticeName, ref INoticeBase<int> param, out INoticeBase<int> notice)
        {
            CreatorHandler creater = mCreators.GetHandler(noticeName);
            bool applyCreater = param == default;
            notice = applyCreater ? creater?.Invoke(noticeName) : param;//调用消息体对象生成器函数

#if LOG_MODULARS
            "error".Log(param == default && creater == default, "Notice creater is null..".Append(" notice = ", noticeName.ToString()));
            "warning".Log(notice == default, "Brocast notice is null..".Append(" notice = ", noticeName.ToString()));
            "warning".Log(!mNoticeDecorator.ContainsKey(noticeName), string.Format("Notice {0} decorator is empty..", noticeName));
#endif
        }

        private void DuringNotifyModular(int noticeName, ref INoticeBase<int> notice)
        {
            if (notice != default)
            {
                DecoratorHandler decorator = mDecorators.GetHandler(noticeName);
                decorator?.Invoke(noticeName, notice);//调用消息体装饰器函数
                noticeName.Broadcast(notice);//广播模块装饰后的消息
#if LOG_MODULARS
                "log".Log(string.Format("Modular App brodcast {0}", noticeName.ToString()));
#endif
            }
            else
            {
#if LOG_MODULARS
                "log".Log("Notify modular by default notice");
#endif
                noticeName.Broadcast(notice);//直接广播消息
            }
        }

        /// <summary>
        /// 发送模块消息
        /// </summary>
        /// <param name="noticeName">消息名</param>
        /// <param name="param">消息体对象，如不为空，则不调用对应的生成器函数</param>
        /// <returns></returns>
        public INoticeBase<int> NotifyModular(int noticeName, INoticeBase<int> param = default)
        {
            BeforeNotifyModular(noticeName, ref param, out INoticeBase<int> notice);
            DuringNotifyModular(noticeName, ref notice);

            return notice;
        }

        public INoticeBase<int> NotifyModularWithParam<T>(int noticeName, T param = default, IParamNotice<T> notice = default)
        {
            INoticeBase<int> temp = notice != default ? notice as INoticeBase<int> : default;

            BeforeNotifyModular(noticeName, ref temp, out INoticeBase<int> result);

            if (result is IParamNotice<T> noticeWithParam)
            {
                noticeWithParam.ParamValue = param;

                DuringNotifyModular(noticeName, ref result);
            }
            else
            {
#if LOG_MODULARS
                "error".Log("Notice param type do not match..".Append(" notice = ", noticeName.ToString()));
#endif
            }
            return result;
        }

        public void NotifyModularAndRelease(int noticeName, INoticeBase<int> param = default, bool isRelease = true)
        {
            INotice notice = NotifyModular(noticeName, param) as INotice;
            if (isRelease)
            {
                notice?.ToPool();
            }
            else { }
        }
    }
         
}
