# WAP (MSIX) Toolkit

[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Serevo.WapToolkit)](https://www.nuget.org/packages/Storex.Core) 



.NET デスクトップ アプリ を [MSIX](https://learn.microsoft.com/ja-jp/windows/msix/) パッケージ化する場合、以下のドキュメントに記載されているような追加の制限事項があります。この **WAP Toolkit** は Visual Studio の Windows Application Package プロジェクト等でパッケージ化されたアプリを開発する際に有効なツールをまとめたキットです。



[デスクトップ アプリケーションのパッケージ化の準備 (MSIX) - MSIX | Microsoft Learn](https://learn.microsoft.com/ja-jp/windows/msix/desktop/desktop-to-uwp-prepare?source=recommendations)

[Windows でパッケージ化されたデスクトップ アプリが動作するしくみについて - MSIX | Microsoft Learn](https://learn.microsoft.com/ja-jp/windows/msix/desktop/desktop-to-uwp-behind-the-scenes#file-system)



## 設定プロバイダー

冒頭ドキュメントに記載の通り、アプリの設定でよく使用されてきた AppData フォルダ への書込みは別の専用の場所にリダイレクトされキャッシュ扱いとなります。またディレクトリ構造も異なる為  [Upgrade](https://learn.microsoft.com/ja-jp/dotnet/api/system.configuration.applicationsettingsbase.upgrade) メソッドによる異なるバージョン間の設定値の引継ぎは機能しなくなります。



### WapDataContainerSettingsProvider

このクラスは [従来のアプリケーション設定](https://learn.microsoft.com/ja-jp/dotnet/desktop/winforms/advanced/application-settings-for-windows-forms) の書込み先として [パッケージ化されたアプリケーション用の設定](https://learn.microsoft.com/ja-jp/windows/apps/design/app-settings/store-and-retrieve-app-data) のストレージを使用する [SettingsProvider](https://learn.microsoft.com/ja-jp/dotnet/api/system.configuration.settingsprovider) です。また  **WapDataContainerLocalFileHybridSettingsProvider** クラスは内部でパッケージ化されているかどうかを検出し、パッケージ化されていなければ従来の [LocalFileSettingsProvider](https://learn.microsoft.com/ja-jp/dotnet/api/system.configuration.localfilesettingsprovider) として機能するハイブリッド プロバイダです。



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

