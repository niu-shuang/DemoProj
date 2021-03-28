namespace DemoProj
{
    public static class ApiErrorDefine
    {
        public static void DefineError(this ApiErrorInfo errorInfo)
        {
            switch (errorInfo.errorStatus)
            {
                case ErrorStatus.None:
                    errorInfo.actionType = ActionType.None;
                    break;
                case ErrorStatus.ForceUpdate:
                    errorInfo.actionType = ActionType.ForceUpdate;
                    break;
                case ErrorStatus.TimeOut:
                    errorInfo.actionType = ActionType.Retry;
                    break;
                case ErrorStatus.InternalError:
                case ErrorStatus.NoUser:
                case ErrorStatus.BannedUser:
                case ErrorStatus.TransferedUser:
                case ErrorStatus.TemporaryBannedUser:
                case ErrorStatus.DatabaseError:
                case ErrorStatus.QueryError:
                case ErrorStatus.DataMismatch:
                case ErrorStatus.NoClassCall:
                case ErrorStatus.RetryError:
                case ErrorStatus.Maintenance:
                case ErrorStatus.MaintenanceParts:
                case ErrorStatus.RequireAgreement:
                case ErrorStatus.NeedsResourceUpdate:
                case ErrorStatus.Exception:
                    errorInfo.actionType = ActionType.Reboot;
                    break;
                default:
                    errorInfo.actionType = ActionType.ShowErrorMessage;
                    break;
            }
        }

        public enum ErrorStatus
        {
            None = 0,

            TimeOut = 408,

            InternalError = 500,
            // ----------------------------------------------------------------------
            // 2999まで、及び9000以上はコンテンツによらない共通エラーコード.
            // ----------------------------------------------------------------------
            NoUser = 1001,  // ユーザーが存在しない.
            BannedUser = 1002,  // ユーザー停止中.
            TransferedUser = 1003,  // ユーザー引き継ぎ済み.
            TemporaryBannedUser = 1004,  // ユーザー一時停止中.

            DatabaseError = 1100,  // DB処理エラー.
            QueryError = 1101,  // クエリエラー.
            DataMismatch = 1200,  // データ不整合.
            NoClassCall = 1300,  // 存在しないクラス呼び出し.
            RetryError = 1400,  // ダブルポストされたくないAPIの連続呼び出し.

            Maintenance = 2001,  // メンテナンス.
            MaintenanceParts = 2002,  // 部分メンテナンス.
            ForceUpdate = 2101,  // 矯正アップデート.
            RequireAgreement = 2201,  // 利用規約が更新された.

            NeedsResourceUpdate = 2300,  // リソース更新が必要.

            Exception = 9999             //デフォルトの未定義エラー

            // ----------------------------------------------------------------------
            // 3000～8999はコンテンツごとのエラーコード.
            // コンテンツ側用のエラー定義に記述してください
            // ----------------------------------------------------------------------
        }

        public enum ActionType
        {
            None,              //api独自で処理
            ShowErrorMessage,  //Confirm Alertを出すだけ
            Retry,             //retry or reboot
            Reboot,             //rebootのみ
            ForceUpdate        //強制アップデート
        }
    }
}
