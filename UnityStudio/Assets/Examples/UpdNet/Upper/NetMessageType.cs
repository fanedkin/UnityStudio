public class NetMessageType
{
    public enum CmdCode : int
    {
        ConnectToHost = 1,  //连接到主机
        JoinInRoom = 2,  //加入房间
        //PlayerEnterNotify = 2,//玩家进入推送通知

        DisconnectToHost = 3,//断开与主机的连接(一般在客户端关闭时操作)
        //PlayerQuitNotify = 4,//玩家离开推送通知

        ////======================//
        ////在fixUpdate中进行信息同步
        ////=====================//
        //SyncPlayerGlobalInfo = 8,//玩家信息同步（位置，面向），包括枪和头盔等

        //FireBulletCreate = 11, //（开火）创建子弹（子弹面向以及旋转）
        //ReloadMagazine = 12, // 重置弹药库

        //MonsterCreateNotify = 20,//怪物创建
        //MonsterAnimationSyncNotify = 21,//怪物动作切换同步
        //UnitInfoSyncNotify = 22, //单元信息同步（如位置，面向）

        //HitDamage = 23, //伤害通知（玩家子弹击中怪物是由客户端发送击中信息，然后主机处理(扣血并裁定是死亡还是击中并确定击中效果等)并转发回所有客户端，怪物攻击是纯主机计算）

        //PlayerRevive = 24,   //玩家复活

        //CutScenesNotify = 25,   //剧情动画

        //MonsterBulletCretateNotify = 26, //怪物子弹

        //NpcCreateNotify = 27,//npc创建

        //BattleStateSwitchNotify = 28, //战场状态切换

        //GameInfoNotify = 29,//游戏信息

        //GameLogicSwitchNotify = 30, //游戏类型切换

        //GameScoreNotify = 31, //游戏分数消息

        //BattleStatisticalResultsNotify = 100, //战斗统计信息

        //PlayAmbientEffectNotify = 101, //环境音效

        ////===========================大厅相关=========================================
        //LobbyOperationNotify = 1000, //大厅操作更新通知（通知游戏中角色相关数据改变，如如切换武器，切换玩家样式）
    }
}
