# DemoProj

 ・UI
  1画面は１Viewと定義されます。1つのシーンは複数のViewを格納できます。
  Viewはテンプレートがあります。新規製作の際に自動化ツールを使用してください。
  Menu-> Tools-> CreateViewをクリックするエディターウインドウが表示されます。
  例えばHomeというシーンにTestViewを追加したい場合、Viewという項目にTest(Viewを入力する必要はありません)を入力し、Sceneという項目にHomeを入力してCreateViewをクリックしてください。
  ![image](https://github.com/niu-shuang/DemoProj/blob/main/ScreenShots/CreateView0.png)
  
  Viewのprefabが下記のところに生成されます。
  ![image](https://github.com/niu-shuang/DemoProj/blob/main/ScreenShots/CreateView1.png)
  
  Viewにはロジックと表現が分離する設計になってます。prefabにはUIPanelView.csがアタッチされてます、prefab配下の各UIComponentのリファレンスを持っています。またScripts/UI/Views/xxxView/にView名と同じ名前のコントローラースクリプトが生成されます。
   ![image](https://github.com/niu-shuang/DemoProj/blob/main/ScreenShots/CreateView3.png)
   
   prefabを編集した後、コントローラーで設定したいUIにUIComponentをアタッチして、prefabのルートにアタッチされているUIPanelViewでFetch Componentをクリックして。各UIのリファレンスがリストに格納されていることを確認。Generate Binding CodeをクリックしてxxxView_Binding.csにバインディングコードが生成されます。
      ![image](https://github.com/niu-shuang/DemoProj/blob/main/ScreenShots/CreateView2.png)
      
 ・通信
  HTTP通信が実装されてます。
  Scripts/APIs/にapiの定義のサンプルがあります。
       ![image](https://github.com/niu-shuang/DemoProj/blob/main/ScreenShots/ApiDemo.png)
  apiを新規作成するにはAPIBase<TData>を継承してください。TDataはレスポンスで受け取ったデータです。
  リクエストにサーバーに送るデータがある場合、コンストラクタで実装してください。
 
 リクエストの中身(具体的な実装はApiRequestInfoをチェックしてください)：
 field:
     uid : uid string
     param : data string (json化したデータ)
     
 レスポンスを処理した後（DTO）の中身
     succeeded bool　通信が成功したか
     dto TData　(APIで定義したデータの定義クラス)
     errorInfo ApiErrorInfo (エラー情報を格納するクラス）
  
