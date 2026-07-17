# Why 400 tests?
> Written by [Matthew](https://github.com/mf366-dev) at [OceanApocalypseStudios](https://github.com/OceanApocalypseStudios) on July 14, 2026.
> 
> It will take about 1:45 to read this article at 220 WPM _(average reading speed)_.

Before we get into the article, I want to inform you it will be much shorter than the last one, mostly because, currently, the focus is finishing [#60](https://github.com/OceanApocalypseStudios/RedSeaMarkupLanguage/issues/60) and finally moving on to RSML's lexer and respective modularity system.

Today, we have officially hit the following milestone in RSML: 400 test methods... for a single class... and it's still not fully tested.
Before you bring your anti-testing and `Console.WriteLine()` pitchforks forward, let us explain why we have implemented that many test methods...

## Context matters
In this case, context matters a lot. The class that has received this much testing attention is [`ReadOnlyStringBuffer`](../../api/OceanApocalypseStudios.RSML.Sources.ReadOnlyStringBuffer.yml), and it serves as the simplest buffer available in the entirety of the official RSML API, yet 400 tests were only half\* of what was needed to get it to work properly.

At OceanApocalypseStudios, we believe testing buffers and big language components (such as lexers and parsers) is imperative to ease the task of getting them to work correctly. If this means testing a lot, then it's a sacrifice that we believe is worth making.

It's worth mentioning this does **not** mean in any way that we believe testing is only relevant when the components are critical: testing is still important when they're not --- the only difference is that it's even more important when the components are critical.

Following the same rationale, we are **not** saying quantity matters more than quality: RSML could have 9000 tests and could be a broken mess or have 10 tests and work perfectly. We believe a good combination of both is the best compromise a developer can choose.

<p style="font-size: 82%;">*This is an assumption, based on the current amount of test methods and the amount of methods that are yet to be tested.</p>

## Expecting more tests?
One word: **yes**. Not because we love writing tests, but because there are still  methods that are publicly exposed yet are fully untested by now. Hopefully, they won't stay that way for much longer, and I'll be happy to write a new article once we can finally mark [#60](https://github.com/OceanApocalypseStudios/RedSeaMarkupLanguage/issues/60) as closed.

---

Like I said, this was quite a small article, and a bigger one is to be expected soon.

Thank you for dropping by and reading! :heart:
