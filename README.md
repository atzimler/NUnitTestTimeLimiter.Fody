![Icon](https://raw.github.com/atzimler/NUnitTestTimeLimiter.Fody/master/Icons/Icon.png)

## NUnitTestTimeLimiter.Fody

Automatically apply Timeout attribute to TestFixture classes.</summary>


## Why?

Fody add-in for weaving NUnit Timeout tags on the TestFixture classes with a global maximum.
Depending on your setting the slow unit tests are going to fail after the timeout period.
      
The goal is to catch all incorrectly written NUnit tests and to force them to execute quickly.
This will provide the possibility to use the test set in TDD scenario.
By not installing the weaver into some of the test assemblies, you can build integration unit tests,
which you can leave out from the TDD process and run them only on your build server.

## Why not assembly level Timeout attribute?

* Has no support for overriding developer preference. So if the developer sets the timeout to high value on the class,
the assembly level configuration will be ignored. On the other hand, by not having the configuration in an assembly it is easy to
enforce a value by the build server, without modifying the source code.

Beside the main point, it is
* Easy to forget,
* Easy to adjust accidentally in a multiple developer environment.

## Configuration (Changing timeout value)

The Default configuration of
```xml
  <NUnitTestTimeLimiter />
```

Is equivalent to 
```xml
  <NUnitTestTimeLimiter TimeLimit="2000" />
```

Feel free to adjust it to your need. It is the time interval parameter supplied to the Timeout attribute, which is interpreted in milliseconds.

