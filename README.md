# YAQAAP
Yet another question and answer platform

.NET, C#, Azure, NoSQL

Demo at http://yaqaap.com

# Install

- Git clone this repository.

- Run install dependencies with bower.

```
bower install
```

- Update your azure storage key account in Yaqaap/web.config. Ex :

```
<add key="storage" value="DefaultEndpointsProtocol=https;AccountName=yaqaap;AccountKey=BOVi0PPafizyc/VWvSkjv6/iDrDceILciqHGMkZEZMTI148/PW45ZtApvdVQ+gLI1S5KDZdd7uw80NW5B6nkmQ==" />
```

# Deploy

Yaqaap use Kudu to deploy into Azure Website. It download all dependency directly.
Just right-click on Yaqapp project from Visual Studio, and click Publish.
