# Benchmarks
At OceanApocalypseStudios, we value performance.

Below is a comparison between the first benchmark of v2.0.0-prerelease8 and the latest one.

> [!WARNING]
> The benchmarks sometimes "lie". Right now, the data is generated during the benchmark's global setup, so the benchmark
> will be different everytime. We're working on this.

## The Before
<details>
	<summary>Show/Hide Benchmark</summary>
	<table style="text-align: right; border-collapse: collapse;">
		<tr style="text-align: center">
			<th>
				Friendly Name
			</th>
			<th>
				Method
			</th>
			<th>
				Mean (ns)
			</th>
			<th>
				Error (ns)
			</th>
			<th>
				Gen 0
			</th>
			<th>
				Gen 1
			</th>
			<th>
				Gen 2
			</th>
			<th>
				Allocations (B)
			</th>
		</tr>
		<tr>
			<td style="text-align: left;">
				Creation of a new evaluator and accessing its `Content` property (1 line)
			</td>
			<td style="text-align: left;">
				ContentProperty SmallContent
			</td>
			<td>
				279.1
			</td>
			<td>
				722.46
			<td>
				0.2789
			</td>
			<td>
				-
			</td>
			<td>
				-
			</td>
			<td>
				584
			</td>
		</tr>
		<tr>
			<td style="text-align: left;">
				Creation of a new evaluator and accessing its `Content` property (10000 lines)
			</td>
			<td style="text-align: left;">
				ContentProperty LargeContent
			</td>
			<td>
				405,550.6
			</td>
			<td>
				117,660.61
			<td>
				110.8398
			</td>
			<td>
				110.8398
			</td>
			<td>
				110.8398
			</td>
			<td>
				350181
			</td>
		</tr>
		<tr>
			<td style="text-align: left;">
				Evaluating 1 line of RSML
			</td>
			<td style="text-align: left;">
				Evaluate SmallContent
			</td>
			<td>
				5,287.5
			</td>
			<td>
				378.29
			<td>
				2.0752
			</td>
			<td>
				-
			</td>
			<td>
				-
			</td>
			<td>
				4352
			</td>
		</tr>
		<tr>
			<td style="text-align: left;">
				Evaluating 100 lines of RSML
			</td>
			<td style="text-align: left;">
				Evaluate MediumContent
			</td>
			<td>
				98,444.7
			</td>
			<td>
				40,498.33
			<td>
				40.6494
			</td>
			<td>
				-
			</td>
			<td>
				-
			</td>
			<td>
				85256
			</td>
		</tr>
		<tr>
            <td style="text-align: left;">Evaluating 10000 lines of RSML</td>
            <td style="text-align: left;">Evaluate LargeContent</td>
            <td>11,233,432.8</td>
            <td>12,026,706.66</td>
            <td>4015.6250</td>
            <td>109.3750</td>
            <td>109.3750</td>
            <td>8747973 B</td>
        </tr>
        <tr>
            <td style="text-align: left;">Evaluating 500 lines of RSML, but with mixed statements</td>
            <td style="text-align: left;">Evaluate ComplexContent</td>
            <td>1,049,512.7</td>
            <td>364,556.17</td>
            <td>451.1719</td>
            <td>-</td>
            <td>-</td>
            <td>943960 B</td>
        </tr>
        <tr>
            <td style="text-align: left;">Checking if a short line is a comment</td>
            <td style="text-align: left;">IsComment True Medium</td>
            <td>109.3</td>
            <td>28.13</td>
            <td>0.1109</td>
            <td>-</td>
            <td>-</td>
            <td>232 B</td>
        </tr>
        <tr>
            <td style="text-align: left;">Checking if an extremely short line is a comment</td>
            <td style="text-align: left;">IsComment True Small</td>
            <td>111.9</td>
            <td>65.24</td>
            <td>0.1109</td>
            <td>-</td>
            <td>-</td>
            <td>232 B</td>
        </tr>
        <tr>
            <td style="text-align: left;">Checking if a short line is a comment</td>
            <td style="text-align: left;">IsComment False Medium</td>
            <td>124.9</td>
            <td>71.19</td>
            <td>0.1109</td>
            <td>-</td>
            <td>-</td>
            <td>232 B</td>
        </tr>
        <tr>
            <td style="text-align: left;">Checking if an extremely short line is a comment</td>
            <td style="text-align: left;">IsComment False Small</td>
            <td>184.0</td>
            <td>1,105.59</td>
            <td>0.1109</td>
            <td>-</td>
            <td>-</td>
            <td>232 B</td>
        </tr>
	</table>
</details>

## The After
<details>
	<summary>Show/Hide Benchmark</summary>
	<table style="text-align: right; border-collapse: collapse;">
		<tr style="text-align: center">
			<th>
				Friendly Name
			</th>
			<th>
				Method
			</th>
			<th>
				Mean (ns)
			</th>
			<th>
				Error (ns)
			</th>
			<th>
				Gen 0
			</th>
			<th>
				Allocations (B)
			</th>
		</tr>
		<tr>
			<td style="text-align: left;">
				Accessing an evaluator's `Content` property (1 line)
			</td>
			<td style="text-align: left;">
				ContentProperty SmallContent
			</td>
			<td>
				0.5433
			</td>
			<td>
				4.7374
			<td>
				-
			</td>
			<td>
				-
			</td>
		</tr>
		<tr>
			<td style="text-align: left;">
				Accessing an evaluator's `Content` property (10000 line)
			</td>
			<td style="text-align: left;">
				ContentProperty LargeContent
			</td>
			<td>0.0001</td>
			<td>
				0.0001
			<td>
				-
			</td>
			<td>
				-
			</td>
		</tr>
		<tr>
			<td style="text-align: left;">
				Evaluating 1 line of RSML
			</td>
			<td style="text-align: left;">
				Evaluate SmallContent
			</td>
			<td>
				5,047.466
			</td>
			<td>
				2,067.6529
			<td>
				1.2512
			</td>
			<td>
				2624
			</td>
		</tr>
		<tr>
			<td style="text-align: left;">
				Evaluating 100 lines of RSML
			</td>
			<td style="text-align: left;">
				Evaluate MediumContent
			</td>
			<td>
				110,423.043
			</td>
			<td>
				62,391.6186
			<td>
				30.5176
			</td>
			<td>
				63904
			</td>
		</tr>
		<tr>
            <td style="text-align: left;">Evaluating 10000 lines of RSML</td>
            <td style="text-align: left;">Evaluate LargeContent</td>
            <td>9,369,561.458</td>
            <td>8,073,274.0617</td>
            <td>3078.1250</td>
            <td>6440224</td>
        </tr>
        <tr>
            <td style="text-align: left;">Evaluating 500 lines of RSML, but with mixed statements</td>
            <td style="text-align: left;">Evaluate ComplexContent</td>
            <td>1,302,738.4766</td>
            <td>402,971.8666</td>
            <td>277.3438</td>
            <td>580000</td>
        </tr>
        <tr>
            <td style="text-align: left;">Checking if a short line is a comment</td>
            <td style="text-align: left;">IsComment True Medium</td>
            <td>7.858</td>
            <td>2.828</td>
            <td>-</td>
            <td>-</td>
        </tr>
        <tr>
            <td style="text-align: left;">Checking if an extremely short line is a comment</td>
            <td style="text-align: left;">IsComment True Small</td>
            <td>7.615</td>
            <td>3.161</td>
            <td>-</td>
            <td>-</td>
        </tr>
        <tr>
            <td style="text-align: left;">Checking if a short line is a comment</td>
            <td style="text-align: left;">IsComment False Medium</td>
            <td>24.107</td>
            <td>4.930</td>
            <td>-</td>
            <td>-</td>
        </tr>
        <tr>
            <td style="text-align: left;">Checking if an extremely short line is a comment</td>
            <td style="text-align: left;">IsComment False Small</td>
            <td>5.4572</td>
            <td>2.7231</td>
            <td>-</td>
            <td>-</td>
        </tr>
	</table>
</details>

## Our Next Goals for Performance
We plan on reducing the amount of allocated bytes, as well as reducing the time it takes for a large buffer.

However, considering most RSML files are up to 100 lines long, these times are good enough for a pre-release.

In the future, we may create an external package called `RSML.Performance` which will contain toolchain components as
`ref structs`, at the cost of less control over the toolchain (such as not being able to use middlewares).
