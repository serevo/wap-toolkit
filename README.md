# WAP (MSIX) Toolkit

[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Serevo.WapToolkit)](https://www.nuget.org/packages/Serevo.WapToolkit) 



.NET デスクトップ アプリ を [MSIX](https://learn.microsoft.com/ja-jp/windows/msix/) パッケージ化する場合、以下のドキュメントに記載されているような追加の制限事項があります。この **WAP Toolkit** は Visual Studio の Windows Application Package プロジェクト等でパッケージ化されたアプリを開発する際に有効なツールをまとめたキットです。



[デスクトップ アプリケーションのパッケージ化の準備 (MSIX) - MSIX | Microsoft Learn](https://learn.microsoft.com/ja-jp/windows/msix/desktop/desktop-to-uwp-prepare?source=recommendations)

[Windows でパッケージ化されたデスクトップ アプリが動作するしくみについて - MSIX | Microsoft Learn](https://learn.microsoft.com/ja-jp/windows/msix/desktop/desktop-to-uwp-behind-the-scenes#file-system)



## アプリケーション設定

冒頭ドキュメントに記載の通り、アプリの設定でよく使用されてきた AppData フォルダ への書込みは別の専用の場所にリダイレクトされます。[従来のアプリケーション設定](https://learn.microsoft.com/ja-jp/dotnet/desktop/winforms/advanced/application-settings-for-windows-forms) を使用している場合、パッケージのバージョンアップ時に設定ファイルの読み書き先ルートが新しく生成され、前のバージョンの設定値は使用できなくなります。これは [Upgrade](https://learn.microsoft.com/ja-jp/dotnet/api/system.configuration.applicationsettingsbase.upgrade) メソッドも同様です。ただし、アプリの実行可能ファイル (.exe) が厳密名用にアセンブリ署名されている場合、設定ファイルの読み書き先ルートは維持され、これらの問題に発生しません。次の幾つかのツールは、何らかの理由により厳密名用にアセンブリ署名することができない場合の回避策として役立ちます。



### WapDataContainerSettingsProvider

このクラスは [従来のアプリケーション設定](https://learn.microsoft.com/ja-jp/dotnet/desktop/winforms/advanced/application-settings-for-windows-forms) の書込み先として [パッケージ化されたアプリケーション設定](https://learn.microsoft.com/ja-jp/windows/apps/design/app-settings/store-and-retrieve-app-data) のストレージを使用する [SettingsProvider](https://learn.microsoft.com/ja-jp/dotnet/api/system.configuration.settingsprovider) です。また  **WapDataContainerLocalFileHybridSettingsProvider** クラスは内部でパッケージ化されているかどうかを検出し、パッケージ化されていなければ従来の [LocalFileSettingsProvider](https://learn.microsoft.com/ja-jp/dotnet/api/system.configuration.localfilesettingsprovider) として機能するハイブリッド プロバイダです。



``` VB
<SettingsProvider(GetType(WapDataContainerLocalFileHybridSettingsProvider))>
Partial Class MySettings
    Inherits ApplicationSettingsBase
End Class
```

``` CS
[SettingsProvider(typeof(WapDataContainerLocalFileHybridSettingsProvider))]
partial class MySettings :　ApplicationSettingsBase
{
}
```



### WapConfigurationManagerIntegration

この静的クラスの `MigrateUnsignedExeConfiguration` メソッドは、厳密名用にアセンブリ署名されていないアプリの [従来のアプリケーション設定](https://learn.microsoft.com/ja-jp/dotnet/desktop/winforms/advanced/application-settings-for-windows-forms) の読み書き先ルートについて、パッケージのバージョンアップによって変更される前のルートを特定し、配下のファイルを新しいルートに全てコピーします。通常、このメソッドは設定値がはじめに読み込まれるよりも前に呼び出します。

