## 取得元數據 和 模板語法 
```
1.{{ 1 + 1 }}
2.<span v-for="i in 3">{{ i }} </span>
3.{{ $page }}
```
1. {{ 1 + 1 }}
2. <span v-for="i in 3">{{ i }} </span>
3. {{ $page }}


## Escaping
::: v-pre
`{{ This will be displayed as-is }}`
:::

## 使用组件
```
.
└─ .vuepress
   └─ components
      └─ demo-1.vue
```

::: v-pre
`引用 <demo-1/> 組件`
::: 

<demo-1/>


### Badge <Badge text="beta" type="warn"/> <Badge text="0.10.1+" vertical="middle"/>
```
### Badge <Badge text="beta" type="warn"/> <Badge text="0.10.1+"/>
```

### Badge 混搭 hyper-link
 [<Badge text="0.10.1+" vertical="middle"/>](https://github.com/thonyl19/MyKata/blob/master/%E8%8B%B1%E6%96%87.md)

 
