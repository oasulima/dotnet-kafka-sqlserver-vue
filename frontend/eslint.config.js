import eslint from '@eslint/js';
import eslintPluginVue from 'eslint-plugin-vue';
import globals from 'globals';
import typescriptEslint from 'typescript-eslint';
import stylisticJs from '@stylistic/eslint-plugin-js'

export default typescriptEslint.config(
  { ignores: ['*.d.ts', '**/coverage', '**/dist', '**/v1.d.ts'] },
  {
    extends: [
      eslint.configs.recommended,
      stylisticJs.configs['all-flat'],
      ...typescriptEslint.configs.recommendedTypeChecked,
      ...eslintPluginVue.configs['flat/essential'],
    ],
    files: ['**/*.{ts,vue}'],
    languageOptions: {
      ecmaVersion: 'latest',
      sourceType: 'module',
      globals: globals.browser,
      parserOptions: {
        parser: typescriptEslint.parser,
        projectService: true,
        tsconfigRootDir: import.meta.dirname,
        extraFileExtensions: [".vue"]
      },
    },
    rules: {
      "@typescript-eslint/no-unused-vars": "off",
      "vue/no-dupe-keys": 'off',
      "@typescript-eslint/no-explicit-any": 'off',
      "vue/multi-word-component-names": 'off',
      "@typescript-eslint/no-unused-expressions": 'off',
      "no-prototype-builtins": 'off',
      "@typescript-eslint/no-unsafe-member-access": 'off',
      "@typescript-eslint/no-unsafe-call": "off",
      "@typescript-eslint/no-unsafe-assignment": "off",
      "@typescript-eslint/no-redundant-type-constituents": "off",
      "@typescript-eslint/no-unsafe-argument": 'off',
      "@typescript-eslint/no-unsafe-return": "off",
      "@typescript-eslint/no-unnecessary-type-assertion": "off",
      "@typescript-eslint/no-floating-promises": "error",
      "@stylistic/js/quotes": ["error", "single"],
      "@stylistic/js/object-curly-spacing": 'off',
      "@stylistic/js/padded-blocks": 'off',
      "@stylistic/js/indent": 'off',
      "@stylistic/js/eol-last": 'off',
      "@stylistic/js/space-before-function-paren": 'off',
      "@stylistic/js/lines-around-comment": 'off',
      "@stylistic/js/quote-props": 'off',
      "@stylistic/js/function-paren-newline": "off",
      "@stylistic/js/function-call-argument-newline": "off",
      "@stylistic/js/multiline-ternary": 'off',
      "@stylistic/js/multiline-comment-style": 'off',
      "@stylistic/js/lines-between-class-members": 'off',
      "@stylistic/js/object-property-newline": 'off',
      "@stylistic/js/no-extra-parens": 'off',
      "@stylistic/js/dot-location": "off",
      "@stylistic/js/array-element-newline": 'off',
      "@stylistic/js/brace-style": 'off',
      "@stylistic/js/function-call-spacing": 'off',
      "@stylistic/js/operator-linebreak": 'off',
      "@stylistic/js/array-bracket-newline": 'off',
      "@stylistic/js/implicit-arrow-linebreak": 'off',
      "@stylistic/js/newline-per-chained-call": 'off',
      "@stylistic/js/wrap-regex": 'off'
    },
  }
);