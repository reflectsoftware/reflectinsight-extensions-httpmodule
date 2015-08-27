# ReflectInsight-Extensions-HttpModule

[![Build status](https://ci.appveyor.com/api/projects/status/github/reflectsoftware/reflectinsight-extensions-httpmodule?svg=true)](https://ci.appveyor.com/project/reflectsoftware/reflectinsight-extensions-HttpModule)
[![Release](https://img.shields.io/github/release/reflectsoftware/reflectinsight-extensions-HttpModule.svg)](https://github.com/reflectsoftware/reflectinsight-extensions-HttpModule/releases/latest)
[![NuGet Version](http://img.shields.io/nuget/v/reflectsoftware.insight.extensions.httpmodule.svg?style=flat)](http://www.nuget.org/packages/ReflectSoftware.Insight.Extensions.HttpModule/)
[![NuGet](https://img.shields.io/nuget/dt/reflectsoftware.insight.extensions.httpmodule.svg)](http://www.nuget.org/packages/ReflectSoftware.Insight.Extensions.HttpModule/)
[![Stars](https://img.shields.io/github/stars/reflectsoftware/reflectinsight-extensions-HttpModule.svg)](https://github.com/reflectsoftware/reflectinsight-extensions-HttpModule/stargazers)

## Overview ##

We've added support for the Http Module. This allows you to leverage your current investment in this, but leverage the power and flexibility that comes with the ReflectInsight viewer. You can view your Http messages in real-time, in a rich viewer that allows you to filter out and search for what really matters to you.

## Benefits of ReflectInsight Extensions ##

The benefits to using the Insight Extensions is that you can easily and quickly add them to your applicable with little effort and then use the ReflectInsight Viewer to view your logging in real-time, allowing you to filter, search, navigate and see the details of your logged messages.

## Getting Started

```powershell
Install-Package ReflectSoftware.Insight.Extensions.HttpModule
```

Then in your web.config file, add the following configuration sections:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <configSections>
    <section name="insightSettings" type="ReflectSoftware.Insight.ConfigurationHandler,ReflectSoftware.Insight" />
  </configSections>

  <insightSettings>
    <baseSettings>
      <configChange enabled="true"/>
      <enable state="all"/>
      <propagateException enabled="false"/>
      <global category="ReflectInsight"/>
      <senderName name="Your_Application_Name"/>
      <exceptionEventTracker time="20"/>
      <!--<debugMessageProcess enabled="true" />-->
    </baseSettings>
    
    <httpModule>
      <properties name="myDebugHttpProperties">
        <userEnterEnterMethod value="true"/>
        <ignoreUrlsParts>
          <!-- will ignore urls that have any part of its path with any key words listed below -->
          <part name="/somepagetoignore.aspx"/>
          <part name="/someotherpagetoignore"/>
          <part name=".htm"/>
          <part name=".html"/>
          <part name=".css"/>
          <part name=".js"/>
          <part name=".gif"/>
          <part name=".png"/>
          <part name=".jpg"/>
          <part name=".bmp"/>
          <part name=".ico"/>
          <part name=".swf"/>
        </ignoreUrlsParts>
        <ignoreUsername value="false"/>
        <ignoreHeader all="false">
          <!-- will ignore these header values -->
          <parameter name="Connection"/>
          <parameter name="Host"/>
        </ignoreHeader>
        <ignoreQueryString all="false">
          <!-- will ignore these query string parameters -->
          <parameter name="name"/>
          <parameter name="location"/>
        </ignoreQueryString>
        <ignoreFormElements all="false">
          <!-- will ignore these form elements -->
          <element name="__VIEWSTATE"/>
          <element name="__EVENTVALIDATION"/>
          <element name="Password"/>
        </ignoreFormElements>
        <ignoreCookies all="false">
          <!-- will ignore these cookie names -->
          <cookie name="ASP.NET_SessionIdX"/>
        </ignoreCookies>
      </properties>
    </httpModule>

    <extensions>
      <extension name="riHttpModule" instance="httpModule" properties="myDebugHttpProperties" enabled="true"/>
    </extensions>

    <exceptionManagement mode="on">
      <publisher mode="on" name="EventPublisher" type="RI.Utils.ExceptionManagement.ExceptionEventPublisher, ReflectInsight.Insight" applicationName="ReflectInsight" />
    </exceptionManagement>

    <!-- Listener Groups -->
    <listenerGroups active="Debug">
      <group name="Debug" enabled="true" maskIdentities="false">
        <destinations>
          <destination name="Viewer" enabled="true" details="Viewer"/>
        </destinations>
      </group>
    </listenerGroups>

    <!-- Log Manager -->
    <logManager>
      <instance name="httpModule" category="HttpModule"/>
    </logManager>
  </insightSettings>
  
  <system.web>
    <compilation targetFramework="4.0" debug="true" />
    <httpRuntime />
    <!-- Used for application pools that support ASP.NET classic mode -->
    <!--<httpModules>
      <add name="RIHttpModule" type="ReflectSoftware.Insight.Extensions.HttpModule.RIHttpModule, ReflectSoftware.Insight.Extensions.HttpModule"/>
    </httpModules>-->
    <sessionState timeout="20" />
    <authentication mode="Windows" />
  </system.web>
  
  <system.webServer>
    <modules runAllManagedModulesForAllRequests="true">
      <!-- Used for application pools that support ASP.NET integrated/pipeline mode -->
      <add name="RIHttpModule" type="ReflectSoftware.Insight.Extensions.HttpModule.RIHttpModule, ReflectSoftware.Insight.Extensions.HttpModule" />      
    </modules>
  </system.webServer>
</configuration>

```

Additional configuration details for the ReflectSoftware.Insight.Extensions.HttpModule logging extension can be found [here](https://reflectsoftware.atlassian.net/wiki/display/RI5/HttpModule+Extension).

## Additional Resources

[Documentation](https://reflectsoftware.atlassian.net/wiki/display/RI5/ReflectInsight+5+documentation)

[Knowledge Base](http://reflectsoftware.uservoice.com/knowledgebase)

[Submit User Feedback](http://reflectsoftware.uservoice.com/forums/158277-reflectinsight-feedback)

[Contact Support](support@reflectsoftware.com)

[ReflectSoftware Website](http://reflectsoftware.com)
