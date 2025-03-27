<lane orientation="vertical"
      horizontal-content-alignment="middle"
      opacity="1">
    <banner background={@Mods/StardewUI/Sprites/BannerBackground}
            background-border-thickness="48,0"
            padding="12"
            text="Get your random number" />
    <frame layout="700px content"
           background={@Mods/StardewUI/Sprites/MenuBackground}
           border={@Mods/StardewUI/Sprites/MenuBorder}
           border-thickness="36,36,40,36"
           padding="32,24">
        <lane layout="stretch content" orientation="vertical">
            <form-row title="Enter text:">
                <textinput layout="250px 48px"
                          margin="-6, 0, 0, 0"
                          max-length="100"
                          text={<>Text} />
            </form-row>
            <button text="Generate Random Number" 
                    click=|GenerateRandom()|
                    margin="0,8" />
            <label text={RandomNumberText}
                   margin="0,8"
                   shadow-alpha="0.8"
                   shadow-offset="-2, 2" />
        </lane>
    </frame>
</lane>

<template name="form-row">
    <lane layout="stretch content"
          margin="16,4"
          vertical-content-alignment="middle">
        <label layout="200px content"
               margin="0,8"
               text={&title}
               shadow-alpha="0.8"
               shadow-offset="-2, 2" />
        <outlet />
    </lane>
</template>